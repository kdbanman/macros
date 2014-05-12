# Game Room

## Vision

A multiplayer game communications engine that uses the AoE lock-step synchronization model.
Each server gameroom instance is a "communications room" for clients playing together.
If possible, RESTful style using a stateful message patterns over WebSockets.

Each playable gameroom can be in two parent states: `setup` or `running`.
The first is for players to join, organize themselves, and finalize the configuration.
The second is for the game engine to run.

Each gameroom can be in child states of the two parent states, like iterating and paused.
Further children (like lagging or not lagging as children of paused) are gameroom defined.
The furthest child states are engine-specific, defined using a possible gameroom state transfer description language.

Little (or, better, zero) engine code should be run on the server.
Divergence/cheating detection is interface-based (ex. checksum of game state done client-side and sent to server).
Command conflicts are resolved client-side.

By enforcing full determinism of game state from an initial seed and a set of possible user commands, only command packets need to be shared between clients (and server).

Clients may not mutate their own game state.
Client commands (empty or not) are gathered at the server, verified, and compiled into a master packet.
The master packet is broadcast to each waiting client, where it is integrated into the next iteration.

The game engine and view/controller are responsible for implementing the commands.
The communications module is responsible for controlling client game engine synchronization.

## Constraints

- must detect and react to client divergence of game engine state
    - gamestate hashes generated client-side, compared server-side
- must be protected against user session spoofing
- must be user-id aware
    - guest uuid if none supplied
- must be some means for player to reconnect from disconnection or accidental back button
- must be tolerant to poor/changing latency
- must be tolerant of different and changing engine iteration times
- must be tolerant to dropped command packets

## Contracts

### Server

- enforces existence of session store for active games

### Game

- enforces game engine recognition of invalid setup
    - server will naively try to synchronize everyone and run the engine when told
- enforces full determinism of engine
    - gamestate hash divergence halts game progress
- enforces 2 game engine stages, setup and run
    - multi stage setup pipelines are child states of setup
- enforces engine mutation by single command packet composed of all client commands
    - view/controller is a command sender
    - engine is a command receiver
        - command receipt enforces full state iteration
    - command conflict resolution must be performed client-side
- enforces header data model of comms-related player/game data sent to and from clients
    - (unenforced) engine specific data is the packet body

## Server Side

- authorized put/post to /create url creates gameroom with submitted content
    - submitted content is game engine and engine seed
    - response is the new waiting join/<gameroom_id> url
- client /join?game_id=<game> url query is served gameroom
    - client must receive game engine, seed, and current setup configuration

### Setup Phase

- clients join with a put/post of username (default uuid if none)
    - new client assigned an open socket
- gameengine setup commands are received async from clients and pushed out
    - first-come-first-served semantics are fine for setup stage
    - setup configuration is allowed before all players have joined
    - full configuration state is sent and broadcast with every change
        - relatively infrequent, so redundancy and data weight is ok
    NOTE: cannot just use setup commands, need full state.  players joining after some configuration is performed would need all commands in order.  just keep setup configuration minimal.
- server waits for all players to commit readiness

### Running Phase

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

## Client Side
 

## Command Packets

Command packets include game engine commands as well as timing control data (latency, framerate) and gamestate checksum.

For motivation, read the AoE article.

### Client Packet

- generation
- latency
- framerate

### Master Packet

- iterations_per_command
