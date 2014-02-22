Prioritized TODO
================

- set up file structure
- continue at TODO below

Pitch
=====

macroscopic battle.  blur the lines between army of units and amorphous war
creature.  control your war blob with pheremones.  protect your base.  kills
mean your war blob grows faster.

player picks/makes a level.  all players join, pick factions, draw base, and
draw initilal unit drop area.  game starts, units start dropping, player
sees other players build form, manipulates own drop area to respond.

any time, player draws movement/attack pheremone trail(s) from his units to a
destination to trigger the movement chain reaction.  same-player units merge
together, combining strength, whenever there is a movement collision.  merged
units cannot split.

moving unit leaves behind trail of movement pheremone to call neighbors along.
units of different factions automatically fight.

units of different factions fight when they are neighbors and when they
collide.  neighbors -> take damage if sum of all neighboring unit strength is
negative (enemy units count negatively).  collide -> stronger cell is the 
victor, no neighborhood dependency.

multiplayer only.  ai can come later.  (minimal control interface -> ai battle
could be another way to play the game:)  game state is completely deterministic
because engine is a pure cellular automata.


Invariant
=========

the following must be true for the duration of the game

- > 1 players
- > 1 factions
- # players >= # factions

Initial
=======

- all invariants must hold
- each player must have the same total base str
- no unit or trans may be present

Win Condition
=============

- single faction's base(s) remain

State
=====

unit - factioned battle units to be controlled by players

- str property is simultaneously attack power and health
- moves in direction of maximum positive neighboring trans delta by filling
  its move buffers
    - if two of the same player's units may merge by moving to the same cell
    - cannot move into cells containing other player's unit
        - cells of differing factions collide, then positive delta remains
        - TODO: cells of same faction, differing player collide?
          TODO: cannot prevent collisions because of automata light speed
    - takes damage if sum(enemy neighbor str) > sum(friendly neighbor str)
        - positive difference is damage taken

trans - movement pheremone for units to follow

- drawable
- left by moving units to encourage other units to move
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

more kills -> higher drop rate. draw drop to shape war creature.

- drawable 
- a drop cell will deposit str for its player
    - either a new unit will be created, or an existing one will be added to
- NOTE: don't do it SC style with a travelling unit placer that is separate
  from the automata ruleset.  avoid global state because of synchronization
  complexity.

Architecture
============

- client index query is served setup.js:
    - player configures a game
        - places BLOCK and BASEPOS
        - chooses start drop and start trans amounts
    - player commits level to server
    - server readies the url with game.js for that game 
    - player directed to that url with game.js

- client game/<game_id> url query is served game.js
    - each player places DROP and commits readiness
    - server waits for all players to commit readiness
    - game begins
    - client combined view and controller renders engine output and serves
      local player commands to server
    - server waits for commands from all clients then broadcasts packet of
      engine calls to each client

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
