#### Prioritized TODO

- think about the relationship between game state seeding and the comms module
    - clearly the comms module must be initialized with a game engine...
    - maybe that init parameter should be a seeded engine?  and the comms module serves the seed to the waiting client comms module?  then the client comms module knows how to stick the seed in the engine hole?  maybe the server comm module should just know how to stick seeds in holes too...

- is omni mode really necessary?

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
- enforces one time game engine initialization by seed object
    - note: multi stage setup pipelines are the responsibility of the setup
            application.  the enginet init is a single object.  period.
- enforces engine mutation by single command object (packet)
- enforces separation of view/controller and engine
    - view/controller is a command sender
    - engine is a command receiver
        - command receipt enforces full state iteration

## Communication

- client index query is served setup.js:
    - player configures a game from config app
    - player commits game seed to server via url api, http post, ...
    - server readies the url with seeded game
    - player directed to that url with game.js
    - player client served game engine, waiting for seed

- client game/<game_id> url query is served game.js
    - server waits for all players to commit readiness
    - game begins, server now locks players to current users
        - other connections are viewers or refused
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
