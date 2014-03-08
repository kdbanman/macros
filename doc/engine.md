#### Prioritized TODO

- init config data spec

## Vision

Game engine with stepwise deterministic evolution with two publicly exposed interfaces.  One interface for view rendering, the other interface for mutation by server.

Identically hosted by server and all clients.

Contains metadata (players, server, creation time, etc) and game state.

## Constraints

- game state evolution must be fully deterministic and identical across all target platforms
- *see backend game engine requirements*

## Contracts

- mutable only by server through client side comms module

## State and Interfaces

### Non-cellular game state

```
{players: [Player,
           Player,
           ...
           Player],
 server: Server,          // see backend.md Server definition
 creation_time: integer,  // time is Server time.
 game: Engine}
           
// Player
{nickname: string,
 faction: integer,
 score: integer,
 drop_rate: integer,
 connection: {latency: integer,
              millis_per_frame: integer}}
```

### Engine

- must be protected from mutation except by server
    - all stateful properties (environment, etc) hidden by closure
    - all getters return values, not references
- render interface must be stable against cell/engine data model changes
    - getter return values should be as unstructured/primitve as possible

```
{options: {move_threshold: integer,
           initial_base_str: integer,
           initial_drop_rate: integer},
 environment: [[cell, cell, ..., cell],
               [cell, cell, ..., cell]],
 droplists: [droplist, droplist, ..., droplist],
 
 applyInitConfig: function(config object),  // callable only once
 applyCommands: function(command packet),
 iterate: function(),

 // below functions are intended for use by view/renderer

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
