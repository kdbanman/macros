Prioritized TODO
================

- set up file structure

Architecture
============

- client index query is served setup.js:
    - player configures a game
        - places BLOCK and BASEPOS
    - player commits level to server
    - server readies the url with game.js for that game 
	
    - player directed to that url with game.js

- client game/GAMEID query is served game.js
    - each player places DROP and commits readiness
    - server waits for all players to commit readiness
    - game begins

Cell
====

    {neighbors: [cell, cell, ..., cell],
     state: {unit: {player: integer,
                    faction: integer,
                    str: integer,
                    move: {x: integer,
                           y: integer,
                           z: integer}},
             trans: [integer, integer, ..., integer],
             base: {player: integer,
                    faction: integer,
                    str: integer}
             drop: {player: integer,
                    active: boolean}}}

State
=====

unit - factioned battle units to be controlled by players

- str property is simultaneously attack power and health
- moves in direction of maximum positive neighboring trans delta by filling
  its move buffers
    - if two of the same player's units may merge by moving to the same cell
    - cannot move into cells containing other player's unit
    - takes damage if sum(enemy neighbor str) > sum(friendly neighbor str)
        - positive difference is damage taken

trans - movement pheremone for units to follow

- drawable
- player's units are only affected by that player's trans
- multiple player's trans may be in one cell
- every cycle, half of the current trans for each player propagates into 
  neighboring cells
    - six neighbors, half is propagated, so neighboring cells take 
      floor ( current trans / 12 ) 
    - dissipates from dropped remainders

base - immovable base to be protected from enemy players

- str property is analogous to unit.str, so is attack/damage behaviour
- each player must start with the same total str

drop - unit production location for each player

- drawable
- an active drop cell will deposit str for its player
    - either a new unit will be created, or an existing one will be added to
- HAS GLOBAL STATE
- str drop rate determined globally
- 

Invariant
=========

the following must be true for the duration of the game

- > 1 players
- > 1 factions
- # players >= # factions
- > 0 drop cell per player
- exactly 1 active drop cell per player

Initial
=======

- each player must have the same total base str
- no unit or trans may be present

