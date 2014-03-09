## Development Reference

Running game engine architecture: [AoE 28.8](http://www.gamasutra.com/view/feature/3094/1500_archers_on_a_288_network_.php)

Websocket game with "rooms": [BrowserQuest](https://github.com/mozilla/BrowserQuest)

## About the Docs

###[meta.md](meta.md)

- Describes development model
- Tracks current vertical slice

# Requirements

- Current VSLICE TODOs are listed per file in order of priority

## Server and Communications

###[webapp.md](webapp.md)

- include route notes from book
    - register and login routes

- figure out which routes are necessary for VSLICE

- specify active game database
    - should be an in memory database for potential sharding
    - define active game data model

###[gameroom.md](gameroom.md)

- 'setup' stage for things like faction choice, start location choice may be better as a separate route/app.  does it make sense to organize everyone in an async environment, then transition to a sync one?
    - VSLICE: just an ugly html form that sends events and responds to them (server is still master)

- define Player data and Command data sent to client
    - see engine.md
    - taken from webapp.State
    - leave spot for Engine stuff

## Macros Game

###[game.md](game.md)
###[engine.md](engine.md)
###[setup.md](setup.md)

# Prototype

- /register is a single text box.  that creates a user
- /login is a single text box.  that logs him in or fails
