## Client-Side API

Programmer using the API is referred to as "maker," short for game maker.

```
gameroom.loop = function myLoop() { ... }
```

- `myLoop` is a maker-defined function called by gameroom's sync controller.
- Game state must not be mutated/augmented within this function.
- Intended mainly for drawing and interpreting UI.

```
gameroom.schedule(myCommand)
```

- `myCommand` is a maker-defined command object that will be received by `doCommand` after being collected and sent to the server for broadcast.
- These commands are meant to effect game state mutation/augmentation *after* myCommand is applied within doCommand.

```
gameroom.myPlayerID = gameroom.me
```

- Player/client ID for each client of the gameroom associated with the `player` parameter of the `doCommand` function.
- **Must** not be modified ever.
    - XXX might be better closure protected exposed as getter?
    - XXX maximize protection by having the server append player ids to packets?

```
gameroom.state = { property: myStateObject, property2: ... }
```

- `property` and `myStateObject` together form a key-value pair of game state.
- Each client must agree about what object is dereferenced by any given `property` key.
- Syntax sugar in `doCommand()` serves as an interface to game state object creation

```
gameroom.doCommand = function myDoCommand(myCommand, player, newStateID) { ... }
```

- `myDoCommand` is a maker-defined function called by gameroom's sync controller.
- `myCommand` is a command object exactly as passed to `schedule`.
- `player` is the player ID of the client who scheduled and sent the command.
- `newStateID` is a new ID sent unique to each command that is the same on each client, intended to become the dereferencing address of a possible new game object.
- Game state should be affected by these commands, as they will be recieved in the same order on each client.

# Object creation

Object creation across multiple clients is a problem!
It means each client needs to add to the game state, and they all need to agree about how to talk about that new addition.
This is called addressability, and you're welcome to use your own addressing system in the `game.state` object, but remember to avoid this:

> Client 1: Create a soldier at base 5548, please!  
> Client 2: K!  
> *Client 2 dereferences object 5548*  
> *Client 2 finds a cow instead of a base*  
> *Client 2 vomits up a stacktrace and crashes.*  
> *No one is having fun anymore.*

Here we show an example of the relationship between `loop()`, `schedule()`, and `doCommand()` as they create and use an object (a base) to create yet another object (a soldier).
A tiny part of an RTS `loop()` is shown, and three different versions of `doCommand()` are shown to show you what's going on under the hood.
The three versions have the *exact same behaviour*.

Loop implementation:

```
gameroom.loop = function ()
{
  // somewhere in loop(), we ask if the player has requested to build a base:
  if ( <player dragged and dropped a barracks sprite on to map> )
  {
    var newBaseCmd = {command: "newBase",
                      x: mouse.x,
                      y: mouse.y,
                     };

    gameroom.schedule(newBaseCmd);
  }

  // elsewhere in loop(), we ask if the player has requested to make a soldier
  if ( <player selected a base and clicked new soldier button> )
  {
    var newSoldierCmd = {command: "newSoldier",
                         baseID: selectedBase.stateID
                        };

    gameroom.schedule(newSoldierCmd);
  }
}
```

Lowest-level `doCommand()` implementation:

```
gameroom.doCommand = function (cmd, player, newStateID) {
  
  if (cmd.command === "newBase")
  {
    // create a new base for the command-sending player at the command location
    var base = new Base(player, cmd.x, cmd.y);

    // using the unique identifier supplied by the gameroom server,
    // put that new base in the game state so you can use it later
    gameroom.state[newStateID] = base;
    
    // and, for convenience, tell the new base what it's ID is
    base.stateID = newStateID;

  } else if (cmd.command === "newSoldier")
  {
    // get the location using the base identifier in the command
    var soldierBase = gameroom.state[cmd.baseID];
    var soldierX = soldierBase.x + 5;
    var soldierY = soldierBase.y + 5;

    // create a new soldier for the command-sending player,
    var soldier = new Soldier(player, soldierX, soldierY);

    // and do the same dance with game state
    gameroom.state[newStateID] = soldier;
    soldier.stateID = newStateID;
  }
}
```

Upgraded `doCommand()` implementation, because giant if-else chains are slow and dirty:

```
gameroom.doCommand = function (cmd, player, newStateID) {

  // rather than test for each cmd.command string, treat each one like an event:

  cmd.onCommand("newBase", function ()
  {
    // create a new base
    var base = new Base(player, cmd.x, cmd.y);

    // do the game state dance
    gameroom.state[newStateID] = base;
    base.stateID = newStateID;
  });

  cmd.onCommand("newSoldier", function ()
  {
    // get the location
    var soldierBase = gameroom.state[cmd.baseID];
    var soldierX = soldierBase.x + 5;
    var soldierY = soldierBase.y + 5;

    // create a new soldier
    var soldier = new Soldier(player, soldierX, soldierY);

    // and do the dance
    gameroom.state[newStateID] = soldier;
    soldier.stateID = newStateID;
  });
}
```

Shiny `doCommand()` implementation, because dancing wastes keystrokes.
Notice you can just ignore the newStateID parameter completely!

```
gameroom.doCommand = function (cmd, player) {
  
  // rather than dancing for each event, just return your new object

  cmd.onCommand("newBase", function ()
  {
    // create a new base and return it
    return new Base(player, cmd.x, cmd.y);
  });

  cmd.onCommand("newSoldier", function ()
  {
    // get the location
    var soldierBase = gameroom.state[cmd.baseID];
    var soldierX = soldierBase.x + 5;
    var soldierY = soldierBase.y + 5;

    // create a new soldier and return it
    return  new Soldier(player, soldierX, soldierY);
  });

}
```

### Notes

Unless you're creating a ton of objects and having runaway memory problems, stay away from JavaScript's `delete` operator.
Imagine what happens when you get a command to create a soldier at a base that's been destroyed by an earlier command.
Rather than executing `delete gameroom.state[baseID]`, do something like `gameroom.state[baseID].setDestroyed()`.
Asking a destroyed base to make a soldier may be a problem, but asking `undefined` to make a soldier is a bigger problem.

Are you worried about cheaters sending their own commands from their JavaScript Console?
If so, then an important addition to the would be a little bit of verification.
For example, when processing a `newSoldier` command, you could stop command effects with a timer on each base:

```
if (soldierBase.secondsSinceLastSoldier() >= 5)
{
  return new Soldier(soldierBase.x, soldierBase.y);
} else
{
  console.log("Could not create soldier - not enough time elapsed.");
}
```

Then, even if a nefarious player manages to fool *his* client into creating a soldier using a fake command or custom code, the other clients still won't create that soldier.
As long as you're putting your state objects, like bases and soldiers, into the `gameroom.state` object, the gameroom will detect the desyncronization.
Read about [gameroom desyncronization](TODO.md) to learn how to deal with this.
Google "javascript closure private" to learn how to prevent your gamestate objects from being compromised in the first place.

