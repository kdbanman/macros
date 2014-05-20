## Development Reference

Running game engine architecture: [AoE 28.8](http://www.gamasutra.com/view/feature/3094/1500_archers_on_a_288_network_.php)

Websocket game with "rooms": [BrowserQuest](https://github.com/mozilla/BrowserQuest)

Development model and past VSLICE phases: [meta.md](meta.md)

## Current VSLICE Phase

### VSLICE_0: Game Communications Engine

Game-independent setup, synchronization, and recording locus for multiplayer game instances.

This is (ideally) a framework in which to build a game.
The synchronization/recording runtime is offered as a service for game providers.

A game community could be hosted on a service that deals with users, social, tournaments, rankings, etc.
That game instances could be synchronized on this service.

# TODO

- Current VSLICE TODOs are listed below per file in order of priority
- TODOs outside the scope of VSLICE are in the files themselves

## Server and Communications

### [gameroom.md](gameroom.md)

- continue minimizing gameroom responsibilities
    - do not serve games
        - continue at #TODO
    - do not fully handle resynchronization of disconnected users (back button), just wait for a client to rejoin with the same state hash
        - should there be a state to express, "1 or more players have disconnected," or should this just be part of the lag state?
            - research websockets, find out how likely it is for a 'lagging' player to appear 'disconnected'.  also how likely it is for a 'disconnected' player to appear 'lagging'
            

- define Comms-level room state tree
    - configing
        - full
    - playing
        - running
        - paused
        - lagging

- define all possible state transitions within state tree

- design Comms packets to enable state transitions
    - enumerate communication sequences between client(s) and server
    - define Player data and Command data sent to client
        - see engine.md
        - taken from webapp.State
        - header for Comms, body for Engine stuff

- define active game data model

## Macros Game

### [game.md](game.md)
### [engine.md](engine.md)
### [setup.md](setup.md)

# Prototype

- /register is a single text box.  that creates a user
- /login is a single text box.  that logs him in or fails
