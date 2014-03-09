# Game Room

## Vision

A multiplayer game communications engine that uses the AoE lock-step synchronization model.  Each server gameroom instance is a "communications room" for client gamerooms playing together.

Each gameroom can be in two main states: `setup` or `running`.  The first is for players to join and organize themselves.  The second is for the game engine to run.

By enforcing full determinism of game state from an initial seed and a set of possible user commands, only command packets need to be shared between clients (and server).

Clients may not mutate their own game state.  Client commands (empty or not) are gathered at the server, verified, and compiled into a master packet.  The master packet is broadcast to each waiting client, where it is integrated into the next iteration.

The game engine and view/controller are responsible for implementing the commands.  The communications module is responsible for controlling client game engine synchronization.

## Constraints

- must not allow games to start with invalid setup phase configuration
- must detect client divergence of game engine state
- must be protected against user session spoofing
- must be user-id aware
- must be some means for player to reconnect from disconnection or accidental back button
- must be tolerant to dropped packets and poor/changing latency
- must be tolerant of different and changing engine iteration times

## Contracts

### Server

- enforces existence of session store for active games

### Game

- enforces game engine recognition of invalid setup
- enforces full determinism of engine
- enforces one time game engine initialization by seed object
    - note: multi stage setup pipelines are the responsibility of the setup app
- enforces engine mutation by single command object (packet)
- enforces separation of view/controller and engine
    - view/controller is a command sender
    - engine is a command receiver
        - command receipt enforces full state iteration
- enforces model of player data sent to client
    - (unenforced) engine specific data has a place within the model

## Server Side

- client /create?seed_id=<seed> url query is served gameroom
- client /join?game_id=<game> url query is served gameroom

### Setup Phase

- gameengine setup commands are received async from clients and pushed out
    - first-come-first-served semantics are fine for setup stage
    - game engine
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
