#### Prioritized TODO

- make sure things are actually REQUIREMENTS (see waterfall spec in meta)
- think about the relationship between game state seeding and the comms module
    - clearly the comms module must be initialized with a game engine...
    - maybe that init parameter should be a seeded engine?  and the comms module serves the seed to the waiting client comms module?  then the client comms module knows how to stick the seed in the engine hole?  maybe the server comm module should just know how to stick seeds in holes too...

## Vision

A multiplayer game communications engine that uses the AoE lock-step synchronization model.

By enforcing full determinism of game state from an initial seed and a set of possible user commands, only command packets need to be shared between clients (and server).

Clients may not mutate their own game state.  Client commands (empty or not) are gathered at the server, verified, and compiled into a master packet.  The master packet is broadcast to each waiting client, where it is integrated into the next iteration.

The game engine and view/controller are responsible for implementing the commands.  The communications module is responsible for controlling client game engine synchronization.

## Constraints

- must be protected against user session spoofing
- must detect client divergence
- must be user-id aware
- must be some means for player to reconnect from disconnection or accidental back button
- must be tolerant to dropped packets and poor/changing latency
- must be tolerant of different and changing engine iteration times

## Contracts

- enforces full determinism of engine
- enforces engine mutation by single command object (packet)
- enforces separation of view/controller and engine
    - view/controller is a command sender
    - engine is a command receiver
        - command receipt enforces full state iteration

## Communication

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
