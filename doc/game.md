#### non-VSLICE TODO

- make this a list.  your 'sentences' are silly
 
# Game

Cellular automata RTS with minimal pheremone-style controls.

## Vision

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

units of different factions fight with awareness of neighborhood when an enemy 
is a neighbor.  they fight without neighborhood awareness when they collide.

multiplayer only.  ai can come later.  (minimal control interface -> ai battle
could be another way to play the game:)  game state is completely deterministic
because engine is a pure cellular automata.

- - - -

## Rules

### Conditions

#### Invariant

the following must be true for the duration of the game

- players >= 2 
- factions >= 2
- number of players >= number of factions

#### Initial

- all invariants must hold
- each player must have the same total base str
- no unit or trans may be present

#### Win

- single faction's base(s) remain

### Cell states

#### Unit

factioned battle units to be controlled by players

- str property is simultaneously attack power and health
- moves in direction of maximum positive neighboring trans delta by filling
  its move buffers
    - if two of the same player's units may merge by moving to the same cell
    - cannot move into cells containing other player's unit
        - cells of differing factions collide, then positive delta remains
        - TODO: cells of same faction, differing player collide? cannot prevent collisions because of automata light speed
    - takes damage if sum(enemy neighbor str) > sum(friendly neighbor str)
        - positive difference is damage taken

#### Trans

movement pheremone for units to follow

- drawable
- left by moving units to encourage other units to move
- player's units are only affected by that player's trans
- multiple player's trans may be in one cell
- every cycle, half of the current trans for each player propagates into 
  neighboring cells
    - six neighbors, half is propagated, so neighboring cells take 
      floor ( current trans / 12 ) 
    - dissipates from dropped remainders

#### Base

immovable base to be protected from enemy players

- str property is analogous to unit.str, so is attack/damage behaviour
- each player must start with the same total str

#### Drop

unit production location for each player.  more kills -> higher drop rate. draw drop to shape war creature.

must not be an advantage to place drop early or often.  the shape of the drop zone should be the only concern of the player.

- drawable
- a drop cell will deposit str for its player round robin in the order of draw time
    - either a new unit will be created, or an existing one's str will be added to
    
### Interface

Note: this is not my domain.  References to relevant parts of the software requirements are mentioned.

#### Setup

Must conform to http api for game creation, whether it's url encoded or http put based or socket.io data event

ex: tools to draw level boundaries and base patterns

#### Game

Must conform to the command api for the game engine

- tools to draw trans and drop patterns
    - patterns are drawn to a command stage

##### Command Stage

- stage can be added to while it is open
- all staged commands are committed at once
- immediate mode
    - stage held open by mouse hold or shift key hold
    - stage committed when both are released
- plan mode
    - stage held open until explicit commit by enter key or dedicated button
