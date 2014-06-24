## Development Reference

Running game engine architecture: [AoE 28.8](http://www.gamasutra.com/view/feature/3094/1500_archers_on_a_288_network_.php)

Websocket game with "rooms": [BrowserQuest](https://github.com/mozilla/BrowserQuest)

Development model and past VSLICE phases: [meta.md](meta.md)

## Current VSLICE Phase

### VSLICE_0: Gameroom - Game Communications Engine

Game-independent setup, synchronization, and recording locus for multiplayer game instances.

This is (ideally) a framework in which to build a game.
The synchronization/recording runtime is offered as a service for game providers.

A game community could be hosted on a service that deals with users, social, tournaments, rankings, etc.
That game instances could be synchronized on this service.

For this slice, the service need not scale beyond a single machine.
A centralized controller for a cluster of Gameroom servers could be designed later.

# TODO

- Current major VSLICE TODOs are listed below
- Current minor VSLICE TODOs are in the files themselves with TODO markers
    - Per-TODO notes are below to keep the other documents clean
- TODOs outside the scope of VSLICE are in the files themselves

## Server and Communications

<!-- -->

- find fastest, most cross-browser reliable game state hash algorithm
    - figure out how to store and retrieve UTF-8 JSON in postgres
        - scan CH 7 for JSON queryability as per logan
    - design schema for client hash reports
        - what are the unique things and how are they related?
        - seed id is master unique thing
            - hashes table primary key(seed id, hash1, hash2, hash3, hash4)?
            - table for each hash?
        - timing table referencing hashes, record each timing report
        - should enable (eventual) equality verification of all reported serializations
            - serializations table referencing hashes table
        - one table with a uid for each client report?

<!-- -->

- across all documentation, make sure session ID and client ID are used to refer to server-side *only*, cross-gameroom browser identifier, while player ID and player number are used for gameroom specific id tasks
    - the former should not be exposed client side for security reasons
    - the latter is used server and client-side
    - session id to player number is one to many

<!-- -->

- research if/how other game libraries handle back button remembery stuff
    
### [Target, Vision, Requirements, Architecture](gameroom/highlevel.md)

<!-- -->

- decide if/how maker needs access to joined/connected method
    - join method or special event or something to tell client its ID
    - should there be separate events for "I connected" vs "other connected"?
    * in the background, invisible to maker, gameroom connects (with existing gameroom.state if in LocalStorage) and negotiates player ID
        * in the event of two players accidentally back buttoning (2 open slots waiting for rejoin) some sort of session mechanism would be really nice for not messing with their old IDs and preventing game hijack
           
### [Design and Implementation](gameroom/lowlevel.md)

<!-- -->

- declare browser hash algorithm choice, link to possible stats writeup

<!-- -->

- define all possible state transitions within state tree

<!-- -->

- design Comms packets to enable state transitions
    - enumerate communication sequences between client(s) and server
    - define Player data and Command data sent to client
        - see engine.md
        - taken from webapp.State
        - header for Comms, body for Engine stuff

### [API documentation](gameroom/client.md)

<!-- -->

- gameroom.me
    - integer between 1 and N, where N players
    - if a player leaves the page and comes back, it will still be his number
    - you cannot change this.  if you have a red player and a blue player, and they want to change who is controlling which color, then it is up to you to swap them by `schedule()`ing commands to change your `gameroom.state`

<!-- -->

- add gamroom.lagging()
    - background - ask for a resend

<!-- -->

- add gameroom.desynced() for resync attempt
    - background - engage dump to server
    - advanced use only.
    - do you know what reversible automata are? if so, do you think they matter here?
    - regardless, are you brave enough to try to implement rewind?

<!-- -->

- add gameroom.close() for gameroom closure
    - prevents further gameroom change or rejoin
    - this is not automatically called. if everyone leaves because of lag, and their browsers are elephants, then they can rejoin later!
    - yay sequentially defined state!

## Macros Game

### [game.md](game.md)
### [engine.md](engine.md)
### [setup.md](setup.md)
