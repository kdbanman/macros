# GameRoom

## Gameroom States

Each room tracks current state and logs previous states.
State of each turn determines server packet broadcast to gameroom members.
Unless a client is misbehaving (iterating before receipt), only commands for a single command turn will arrive for the room.

Raw state:

- turn number
    - number of players (expected)
    - socket id 1
        - previous arrival lag/lead
        - latency
        - framerate
        - gamestate hash
        - command body
    - socket id 2, 3, ...
        - <same as sid 1>

Derived state:

- waiting
- started
    - running
        - TODO: substates based on receipt, process, broadcast cycle
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

client-command.js command module exposes `new Command()` and augmentation methods
    - requires client command object (`cmd` in the following) to be augmented with `onCmd` property
    - cmd.onCmd("newBase", function() { var newObj = new ..(); ... return newObj;}
        - requires `onCmd` return to be evaluated:
            - if (clientCommand.onCmd && typeof clientCommand === 'function') newThing = clientCommand.onCmd(); if (typeof newThing !== 'undefined') gameroom.state[newStateID] = newThing;

### From Server

Server packets include a commandID (newStateID), player ID, processed timing control data, and the game command body.

TODO: JSON model

## Transport Libraries

- for socket.io, a single HTTP server accepting PUT to /create for room creation in the `adapter`'s database
        - room mechanics are controlled by `adapter` module, so I'll write/customize one for cassandra or something
            - join full -> error
            - join nonexistent -> error
        - all gamerooms are under the default `Namespace`
            - Namespaces are for multiplexing multiple Sockets (of different Namespaces) across a single transport (engine.io Client)
            - might be useful to have a dumb `/chat` channel that doesn't care about lag state or syncronization for each gameroom using namespaces
        - `io.use(function(socket, next)) middleware useful for room assignment and other handshake steps
            - XXX room assignment maybe, but handshakey stuff might need to be after the 'connection' event.  what's the difference between the two?
    - for engine.io i'll likely be reimplementing socket.io rooms and events in a less maintainable way
    - for einaros/ws, a single HTTP server accepting PUT to /create with many WebSocketServers listening on their own /play/<room_id>
        - a whole server per room may be pretty wasteful
        - just like engine.io, i'll likely be reimplementing rooms and events

## Service Model Comparison

Here various ideas for exposing the gameroom server as a public service are explored.

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

- Avoids cross domain complexity, I think (simpler client and server configurations)
