# Game Room

## Vision

A multiplayer game communications engine that uses the AoE lock-step synchronization model.
Each server gameroom instance is a "communications room" for clients playing together.
Where possible, RESTful style using a stateful message patterns over WebSockets.

Each playable gameroom can be in two parent states: `setup` or `running`.
The first is for players to join, organize themselves, and finalize the configuration.
The second is for the game engine to run.

No engine code should be run on the server.
Divergence/cheating detection is interface-based (ex. checksum of game state done client-side and sent to server).
Game-specific command conflicts are resolved client-side.

By enforcing full determinism of game state from an initial seed and a set of possible user commands, only command packets need to be shared between clients (and server).
Only command packets sent by server may modify game state.

Client commands (empty or not) are gathered at the server, verified, and compiled into a master packet.
The master packet is broadcast to each waiting client, where it is integrated into the coming iteration. (See separation of communication turns in AoE article)

The game engine and view/controller are responsible for implementing the commands.
The communications module is responsible for controlling client game engine synchronization.

## Requirements

- must handle defaultest browser configurations possible
- must persistently record all gameroom activity
- must detect and react to client divergence of game engine state
    - gamestate hashes generated client-side, compared server-side (consensus)
- must be user-id aware
    - guest uuid if none supplied
- must be some means for player to reconnect from disconnection or accidental back button
    - must *not* be responsible for resynchronization of game state (only verification of resync success)
- must be tolerant to poor/changing latency
- must be tolerant of different and changing engine iteration times
- must be tolerant to dropped command packets
- must *not* be responsible for engine-specific command validation

## Service Model Comparison

### Cross Domain Connection

Game is constructed using the framework implementing the client side of gameroom.
Authorized service creates a gameroom, then serves the client the game.
The served game connects to the newly created gameroom.

#### Benefits

- Lightest, simplest server state

### Common Host For iFrame

Game is constructed using the framework implementing the client side of gameroom.
Authorized service creates a gameroom, then serves the client a page with an iframe to contain the game.
The iframe is served the game from my domain.
The game makes a (same origin) connection to the gameroom.

#### Benefits

NOTE: This style is still possible with other models.  Here it is enforced.

- Avoids cross domain complexity (simpler client and server configurations)

## Contracts

### Game

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

### HTTP Gameroom Creation

- authorized put/post to /create url creates gameroom
    - response is the new waiting join/<gameroom_id> url

- authorized get from /archive/<gameroom_id> is returned .json played game log

### WS Gameroom Connection

- client ws:// connection to existing /play/<game_id> url is:
    - connected to gameroom in non-full setup phase
        - client must reach current setup configuration
    - denied connection to gameroom in full setup or run phase
        - diverted to live /watch/<game_id> if possible

#TODO

**TODO iteratively change and separate what's below into state tree, state descriptions**

## Gameroom States

- setup
    - not full
    - full
- run
    - running
    - lagging


### Setup State

- gameengine setup commands are received async from clients and pushed out
    - first-come-first-served semantics are fine for setup stage
    - setup configuration is allowed before all players have joined
    - full configuration state is sent and broadcast with every change
        - relatively infrequent, so redundancy and data weight is ok
    NOTE: cannot just use setup commands, need full state.  players joining after some configuration is performed would need all commands in order.  just keep setup configuration minimal.
- server waits for all players to commit readiness
- final state from setup phase represents the seed for running state

### Running State

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
