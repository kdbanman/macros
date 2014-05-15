# Game Room

## Vision

A multiplayer game communications engine that uses the AoE lock-step synchronization model.
Each server gameroom instance is a "communications room" for clients playing together.
Where possible, RESTful style using a stateful message patterns over WebSockets.

Each playable gameroom can be in two parent states: `setup` or `running`.
The first is for players to join, organize themselves, and finalize the configuration.
The second is for the game engine to run.

Little (or, better, zero) engine code should be run on the server.
Divergence/cheating detection is interface-based (ex. checksum of game state done client-side and sent to server).
Command conflicts are resolved client-side.

By enforcing full determinism of game state from an initial seed and a set of possible user commands, only command packets need to be shared between clients (and server).
Only command packets sent by server may modify game state.

Client commands (empty or not) are gathered at the server, verified, and compiled into a master packet.
The master packet is broadcast to each waiting client, where it is integrated into the coming iteration. (See separation of communication turns in AoE article)

The game engine and view/controller are responsible for implementing the commands.
The communications module is responsible for controlling client game engine synchronization.

## Constraints

- must detect and react to client divergence of game engine state
    - gamestate hashes generated client-side, compared server-side (consensus)
- must be user-id aware
    - guest uuid if none supplied
- must be some means for player to reconnect from disconnection or accidental back button
- must be tolerant to poor/changing latency
- must be tolerant of different and changing engine iteration times
- must be tolerant to dropped command packets

## Service Model

Game is constructed using the framework implementing the client side of gameroom.
Authorized service creates a gameroom, then serves the client the game.
The served game connects to the newly created gameroom.

## Contracts

### Client

- client must already be served game engine

### Game

- enforces game engine recognition of invalid setup
    - server will naively try to synchronize everyone and run the engine when told
- enforces full determinism of engine
    - gamestate hash divergence halts game progress
- enforces 2 main game engine stages, setup and run
    - multi stage setup pipelines are child states of setup
    - lagging is a child state of running because lag detection during asyncronous setup state is impossible
- enforces engine mutation by single command packet composed of all client commands
    - view/controller is a command sender
    - engine is a command receiver
        - command receipt enforces full state iteration
    - command conflict resolution must be performed client-side
- enforces header data model of comms-related player/game data sent to and from clients
    - (unenforced) engine specific data is the packet body

## Server

- authorized put/post to /create url creates gameroom
    - response is the new waiting join/<gameroom_id> url
- client /play/<game_id> url query (over ws://) is connected to gameroom
    - client must reach current setup configuration

#TODO

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
