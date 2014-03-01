#### Prioritized TODO

- read thoroughly
    - make sure things are actually REQUIREMENTS

# Software

## Constraints

- entire system must be tolerant to dropped packets and poor/changing latency
- must be some means for player to reconnect from disconnection or accidental back button
- games must be protected against spoofing and cheating

## Communication

client communication modules use closure-guarded player ids and game ids.
server communication module tries to make sure ip address, game id, and player
ids match as expected to prevent nefarious action.

- client index query is served setup.js:
    - player configures a game
        - places BLOCK and BASE
        - chooses start drop and start trans amounts
        - chooses omni mode or not (possibly for testing only)
    - player commits level to server
    - server readies the url with game.js for that game 
    - player directed to that url with game.js

- client game/<game_id> url query is served game.js
    - each player places DROP and commits readiness
    - server waits for all players to commit readiness
    - game begins, server now denies other game url queries
    - client combined view and controller renders engine output and serves
      local player commands to server
    - server waits for client command packets from all clients
        - server validates each and combines into master command packet
        - server applies master packet engine
        - server iterates engine, adds resulting checksum to master packet
        - server broadcasts master packet to each client
        - server returns to waiting

- omni/<game_id> url query served omni.js
    - for testing
    - can spoof any player id
    - raw paper.js svg view/controller
    - can place drop and commit readiness for all players
    - serves master packets


