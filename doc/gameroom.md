# Game Room

## Target

Indie game makers with programming and framework use experience who want multiplayer from the ground up.
All I want to have to tell them is, "google HTML5 games for beginners."

Big scalers come later.

## Vision

A multiplayer game communications engine that uses the AoE lock-step synchronization model.
Each server gameroom instance is a "communications room" for clients playing together.

The server-side gamerooms have minimal responsibilities - they are communications/recording loci.
They do not run any game code or validate any game commands.
They are of fewest possible states (those derivable from the communications library protocol that are relevant to multiplayer game communications).

Any asynchronous 'setup' phase for different players to join and leave is to be handled BEFORE connecting to a game room.
Once the number of connected clients is equal to the number of expected players, the game room lock-step cycle begins.

By enforcing full determinism of game state from an initial seed and a set of possible user commands, only command packets need to be shared between clients (and server).
Only command packets sent by server may modify game state.

No engine code should be run on the server.
Divergence/cheating detection is interface-based (ex. checksum of game state done client-side and sent to server as part of packet header).
Game-specific command conflicts are resolved client-side.

Client commands (empty or not) are gathered at the server, processed, and compiled into a master packet.
The master packet is broadcast to each waiting client, where it is integrated into the coming iteration. (See separation of communication turns in AoE article)

The game engine and view/controller are responsible for implementing the commands.
The client-side gameroom module is responsible for controlling client game state synchronization and frame draw calls.

## Requirements

- must handle defaultest browser configurations possible
- must persistently record all gameroom activity
    - error states and traces are very important (ex. state divergence)
        - on disconnect or exception, dump as much as possible to server
- must detect and react to client divergence of game engine state
    - gamestate hashes generated client-side, compared server-side (consensus)
- must be user-id aware
    - guest uuid if none supplied
    - XXX part of optional configuration for richer recorded data?
- must be some means for player to reconnect from disconnection or accidental back button
    - must *not* be responsible for resynchronization of game state (only verification of resync success)
- must be tolerant to poor/changing latency
- must be tolerant of different and changing engine iteration times
- must be tolerant to dropped command packets
    - XXX TCP + WebSocket or communications library may handle this?
- must *not* be responsible for engine-specific command validation

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

# Object creation TODO move to client-doc.md

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
The three versions have the *exact same behaviour*:

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
                         x: selectedBase.x + 5,
                         y: selectedBase.y + 5
                        };

    gameroom.schedule(newSoldierCmd);
  }
}
```

Lowest-level `doCommand()` implementation:

```
gameroom.doCommand = function (cmd, player, newStateID) {
  TODO
}
```

Upgraded `doCommand()` implementation:

```
gameroom.doCommand = function (cmd, player, newStateID) {
  TODO
}
```

Shiny `doCommand()` implementation:

```
gameroom.doCommand = function (cmd, player, newStateID) {
  TODO
}
```

## Service Model Comparison

### Upload to Me

Game is constructed using the gameroom framework.
Authorized game maker uploads to my site.
I host and run.

Benefits

- Lowest barrier to entry by far

Cons

- Need an actual webapp to deal with all that shit

### Cross Domain Connection

Game is constructed using the framework implementing the client side of gameroom.
Authorized service creates a gameroom, then serves the client the game.
The served game connects to the newly created gameroom.

Benefits

- Lightest, simplest server state

Cons

- Higher barrier to entry

### Common Host For iFrame

Game is constructed using the framework implementing the client side of gameroom.
Authorized service creates a gameroom, then serves the client a page with an iframe to contain the game.
The iframe is served the game from my domain.
The game makes a (same origin) connection to the gameroom.

Benefits

NOTE: This style is still possible with other models.  Here it is enforced.

- Avoids cross domain complexity (simpler client and server configurations)

## Contracts

### Game

- enforces full determinism of engine
- gamestate hash divergence halts normal lock-step cycle
    - XXX possible resync attempt use case may restart lock-step?
- players joining/leaving (with possible async setup pipelines) must be handled before game room connection
- enforces game state mutation by only by command packets
    - view/controller is a command sender, client gameroom is a command receiver
    - command conflict resolution must be performed client-side
- enforces player reference and object creation/reference by gameroom supplied ids

## Server

### HTTP Gameroom Creation

- authorized put/post to /create url creates gameroom
    - response is the new waiting join/<gameroom_id> url

- authorized get from /archive/<gameroom_id> is returned .json played game log

### WS Gameroom Connection

- client ws:// connection to existing /play/<game_id> url is:
    - connected to gameroom if non-full
        - client must agree with game state hash
    - denied connection to gameroom if running (full -> running) or if game state hash does not agree
        - XXX or if client session does not match a current disconnect slot?

## Gameroom States

- waiting
- started
    - running
        - TODO: substates based on receipt, process, broadcast cycle
    - lagging
    - vacancy


### Waiting

- not full - number of expected players does not equal number of connected players
- server waiting for all (state-agreeing) players to commit readiness by connecting

### Started

- set of connected players equal reaches expected number
- sessions locked - different players may not join in the event of a disconnect
    - XXX hard to imagine how a fresh player could get current game state to join after a disconnect, but maybe this isn't a necessary thing?

Running

- lock-step sync, expected behaviour

TODO: substates based on receipt, process, broadcast cycle

Lagging

- certain threshold since command receipt from 1+ players

Vacancy

- 1+ players disconnected

## Command Packets

### From Client

Command packets include timing control data (latency, framerate), gamestate checksum, and a body for actual game commands.

TODO: JSON model

### From Server

Server packets include a commandID (newStateID), player ID, processed timing control data, and the game command body.

TODO: JSON model
