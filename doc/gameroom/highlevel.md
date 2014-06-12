# Game Room

## Target

Indie game makers with programming and framework experience who want multiplayer from the ground up.
Google "HTML5 games for beginners" and keep in mind the [API documentation](gameroom/client.md).

## Vision

A multiplayer game communications engine that uses the lock-step, command-based synchronization model.
Each server gameroom instance is a "communications room" for 2 or more gameroom clients playing together in sync.

The server-side gamerooms have minimal responsibilities - they are synchronization/recording loci.
They do not run any game code or validate any game commands.
They have the fewest possible states (those derivable from the communications library protocol that are relevant to multiplayer game synchronization).

Any asynchronous 'setup' phase for different players to join and leave is to be handled BEFORE connecting to a gameroom.
The gamestate hash of the first connected client decides what the other clients must agree with in order to connect.
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
- must be tolerant to dropped command packets (see [Two Generals' Problem](http://en.wikipedia.org/wiki/Two_Generals%27_Problem))
- must *not* be responsible for engine-specific command validation

## Contracts for Game

- enforces full determinism of engine
- gamestate hash divergence halts normal lock-step cycle
    - XXX possible resync attempt use case may restart lock-step?
- players joining/leaving (with possible async setup pipelines) must be handled before game room connection
- enforces game state mutation by only by command packets
    - view/controller is a command sender, client gameroom is a command receiver and applier
    - command conflict resolution must be performed client-side
- enforces player reference and object creation/reference by gameroom supplied ids

## Server

The server must have an HTTP API for gameroom management:

- authorized put/post to /create url creates gameroom
    - response is the new waiting join/<gameroom_id> url

- authorized get from /archive/<gameroom_id> is returned .json played game log

The server must have realtime connection acceptance/denial logic per gameroom:

- client realtime connection to existing /play/<game_id> url is:
    - connected to gameroom if
        - gameroom is not full
        - gamestate hash agreement 
        - turn number agreement
    - denied otherwise
    - XXX do we need *re*connection semantics? if client session does not match a current disconnect slot?
