#### Prioritized TODO

- read thoroughly
    - add markers to areas that click as inconsistent and keep reading
- include player id and game id in data models

#### About this document

From top to bottom, across all subsections: high level, general requirements
progressing to low level, specific requirements.  In essence, if you see
something about which you need more detail, look lower in the document.

# Game

## Vision

macroscopic battle.  blur the lines between army of units and amorphous war
creature.  control your war blob with pheremones.  protect your base.  kills
mean your war blob grows faster.

player picks/makes a level.  all players join, pick factions, draw base, and
draw initilal unit drop area.  game starts, units start dropping, player
sees other players build form, manipulates own drop area to respond.

any time, player draws movement/attack pheremone trail(s) from his units to a
destination to trigger the movement chain reaction.  same-player units merge
together, combining strength, whenever there is a movement collision.  merged
units cannot split.

moving unit leaves behind trail of movement pheremone to call neighbors along.
units of different factions automatically fight.

units of different factions fight with awareness of neighborhood when an enemy 
is a neighbor.  they fight without neighborhood awareness when they collide.

multiplayer only.  ai can come later.  (minimal control interface -> ai battle
could be another way to play the game:)  game state is completely deterministic
because engine is a pure cellular automata.

- - - -

## Rules

### Conditions

#### Invariant

the following must be true for the duration of the game

- players >= 2 
- factions >= 2
- number of players >= number of factions

#### Initial

- all invariants must hold
- each player must have the same total base str
- no unit or trans may be present

#### Win

- single faction's base(s) remain

### Cell states

#### Unit

factioned battle units to be controlled by players

- str property is simultaneously attack power and health
- moves in direction of maximum positive neighboring trans delta by filling
  its move buffers
    - if two of the same player's units may merge by moving to the same cell
    - cannot move into cells containing other player's unit
        - cells of differing factions collide, then positive delta remains
        - TODO: cells of same faction, differing player collide? cannot prevent collisions because of automata light speed
    - takes damage if sum(enemy neighbor str) > sum(friendly neighbor str)
        - positive difference is damage taken

#### Trans

movement pheremone for units to follow

- drawable
- left by moving units to encourage other units to move
- player's units are only affected by that player's trans
- multiple player's trans may be in one cell
- every cycle, half of the current trans for each player propagates into 
  neighboring cells
    - six neighbors, half is propagated, so neighboring cells take 
      floor ( current trans / 12 ) 
    - dissipates from dropped remainders

#### Base

immovable base to be protected from enemy players

- str property is analogous to unit.str, so is attack/damage behaviour
- each player must start with the same total str

#### Drop

unit production location for each player.  more kills -> higher drop rate. draw drop to shape war creature.

must not be an advantage to place drop early or often.  the shape of the drop zone should be the only concern of the player.

- drawable
- a drop cell will deposit str for its player
    - either a new unit will be created, or an existing one will be added to
    
TODO: figure out drop semantics
NOTE: don't do it SC style with a travelling unit placer that is separate
  from the automata ruleset.  avoid global state because of synchronization
  complexity.

### Interface

#### Setup

- tools to draw level boundaries and base patterns

#### Game

- tools to draw trans and drop patterns
    - patterns are drawn to a command stage

##### Command Stage

- stage can be added to while it is open
- all staged commands are committed at once
- immediate mode
    - stage held open by mouse hold or shift key hold
    - stage committed when both are released
- plan mode
    - stage held open until explicit commit by enter key or dedicated button

- - - -

# Software

## Constraints

- entire system must be tolerant to dropped packets and poor/changing latency
- must be some means for player to reconnect from disconnection or accidental back button
- game state evolution must be fully deterministic and identical across all target platforms
- external game state mutation may only be through minimal command packets
- games must be protected against spoofing and cheating

- - - -

## Architecture

### Communication

client communication modules use closure-guarded player ids and game ids.
server communication module tries to make sure ip address, game id, and player
ids match as expected to prevent nefarious action.

- client index query is served setup.js:
    - player configures a game
        - places BLOCK and BASE
        - chooses start drop and start trans amounts
        - chooses omni mode or not (possibly for testing only)
    - player commits level to server
    - server readies the url with game.js for that game 
    - player directed to that url with game.js

- client game/<game_id> url query is served game.js
    - each player places DROP and commits readiness
    - server waits for all players to commit readiness
    - game begins, server now denies other game url queries
    - client combined view and controller renders engine output and serves
      local player commands to server
    - server waits for client command packets from all clients
        - server validates each and combines into master command packet
        - server applies master packet engine
        - server iterates engine, adds resulting checksum to master packet
        - server broadcasts master packet to each client
        - server returns to waiting

- omni/<game_id> url query served omni.js
    - for testing
    - can spoof any player id
    - raw paper.js svg view/controller
    - can place drop and commit readiness for all players
    - serves master packets

### Non-cellular game state

```
{players: [player,
           player,
           ...
           player],
 server: {ip: string},
 game: .ngine}
           
// player
{nickname: string,
 faction: integer,
 score: integer,
 drop_rate: integer,
 connection: {ip: string,
              latency: integer,
              millis_per_frame: integer}}
```

### Engine

- must be protected from mutation except by server
    - all stateful properties (environment, etc) hidden by closure
    - all setters must only accept one call per engine instance
        - server sets upon player connection to game.js
    - all getters return values, not references
- render interface must be stable against cell/engine data model changes
    - getter return values should be as unstructured/primitve as possible

```
{options: {move_threshold: integer,
           initial_base_str: integer},
 environment: [[cell, cell, ..., cell],
               [cell, cell, ..., cell]],
 droplists: [droplist, droplist, ..., droplist],
 
 setMoveThreshold: function(integer),
 setInitialBaseStr: function(integer),
 setEnvironment: function(environment),

 applyCommands: function(command packet),
 iterate: function(),

 getUnitPlayer: function(x, y),  // integer
 getUnitFaction: function(x, y), // integer
 getUnitStr: function(x, y),     // integer
 getMoveX: function(x, y),	 // integer
 getMoveY: function(x, y),       // integer
 getMoveZ: function(x, y),       // integer

 getBasePlayer: function(x, y),  // integer
 getBaseFaction: function(x, y), // integer
 getBaseStr: function(x, y),     // integer

 getDropPlayer: function(x, y),  // integer

 getTrans: function(x, y),       // [integer, integer, ... integer]

 getCurrentDrop: function(),     // [[x, y], [x, y], ... [x, y]] (integers)
 getNextDrop: function()}        // [[x, y], [x, y], ... [x, y]] (integers)
```

- unit, base, and drop are not allowed to stack, so single integer return
- trans may stack, so integer array of quantities is returned
    - array index corresponds to player number
- getNext/CurrentDrop returns array of integer coordinates indicating drop sites
    - array index corresponds to player number

### Droplist

- drop system must not depend upon update order of cells
- drop system must always have >= 1 active cell

```
{head: {next: cell,
        previous: null},
 current: cell,
 removeCells: function([cell, cell, ... cell]),
 appendCells: function([element, element, ... element])}
```

- one droplist per player is stored in engine
- doubly linked list for constant time removal/insertion
- embedded into environment for constant time retrieval (if coordinate known)
- head.next always points to oldest active cell
- list may only be appended to (to preserve age-sorted order)

### Cell

- cell state must be cleared to default state before iteration to prevent
  state carry-over from previous generation (next cell state must depend only
  upon current cell state)
 
```
{neighbors: [cell, cell, ..., cell],
 state: {unit: {player: integer,                  // -1
                faction: integer,                 // -1
                str: integer,                     // 0
                move: {x: integer,                // 0
                       y: integer,                // 0
                       z: integer}},              // 0
         trans: [integer, integer, ..., integer], // [0, 0, ... 0]
         base: {player: integer,                  // -1
                faction: integer,                 // -1
                str: integer}                     // 0
         drop: {player: integer,                  // -1
                next: cell,                       // null
                previous: cell}}}                 // null
                
```

- all drop cells for one player comprise an environmentally embedded doubly
  linked list

### Client Command Packet

```
{trans: [{location: {x: integer,
                     y: integer},
          quantity: integer,
          player: integer},
          ...],
 drop: [{location: {x: integer,
                    y: integer},
         erase: boolean},
         ...]}}
```

### Master Command Packet

```
{commands: [client cmd packet, client cmd packet, ... client cmd packet],
 result_hash: integer}
```
