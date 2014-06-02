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

- Current VSLICE TODOs are listed below per file in order of priority
- TODOs outside the scope of VSLICE are in the files themselves

## Server and Communications

### [gameroom.md](gameroom.md)

- put client usecase from notebook in

- decide if/how maker needs access to joined/connected method
    - join method or special event or something to tell client its ID
    - should there be separate events for "I connected" vs "other connected"?
    * in the background, invisible to maker, gameroom connects (with existing gameroom.state if in LocalStorage) and negotiates player ID
        * in the event of two players accidentally back buttoning (2 open slots waiting for rejoin) some sort of session mechanism would be really nice for not messing with their old IDs and preventing game hijack

- clients need to agree on which client is which
    - similar addressability concerns as object creation
    - imagine a maker is detecting clicks on soldiers.  he needs to know which player 'owns' that soldier so his UI can respond accordingly.
        - i.e. the maker needs access to client/player IDs
        - make doCommand(cmd, player, newID)
        - player and newID are gameroom headery/control stuff, cmd is THE MAKERS DON'T TOUCH IT

- try assuming that whatever join/leave async, full-state-sharing bullshit needs to be done CAN be done before gameroom is even involved
    - confirm with logan

- research websocket libraries for state/architectural choices
    - for einaros/ws, a single HTTP server accepting PUT to /create with many WebSocketServers listening on their own /play/<room_id>
    - for TopCloud/socketcluster, configure with 'websocket' transport only (based on engine.io), and ? (see nombo.io for usage and docs)
            
- define Gameroom state tree    
    - find minimal amount of states for a room of websocketers continue at #TODO
        - find out how likely it is for a 'lagging' player to appear 'disconnected'.  also how likely it is for a 'disconnected' player to appear 'lagging'

- define all possible state transitions within state tree

- design Comms packets to enable state transitions
    - enumerate communication sequences between client(s) and server
    - define Player data and Command data sent to client
        - see engine.md
        - taken from webapp.State
        - header for Comms, body for Engine stuff


## Macros Game

### [game.md](game.md)
### [engine.md](engine.md)
### [setup.md](setup.md)
