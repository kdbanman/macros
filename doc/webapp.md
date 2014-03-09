## Vision

A web application to support level creation, gameplay, game replay, and community for Macros.

Free guest players and paid registered users.

## Constraints

- must encourage user registration
- must allow free guest play
    - guest players must not be allowed to do server-resource intensive tasks
- must be as stateless as possible
    - session data must be as minimal and as static as possible
    - application behaviour should be minimally dependent upon session data
- main features must exist on separate routes
    - EX: /play and /replay should NOT be combined under /game

## Contracts

## Routes

Each route has a dedicated specification.md file.  See each for more details.

- /  (index query)

- [/setup](setup.md)
    - game seed design zone

- /games
    - main entry for gameplay
        - create game with seed chosen from list
            - links to /create?seed_id=<seed>
        - join game from list
            - links to /join?game_id=<game>

- [/create?seed_id=<seed>](gameroom.md)
    - server spins up server gameroom instance with seed
    - client served client gameroom instance with seed
    - server connects to client over websocket

- [/join?game_id=<game>](gameroom.md)
    - client served client gameroom instance with seed
    - server connects to client over websocket

- /replay

## State

### Persistent (Global)

User Data

- persistent storage of user data
- will be modified lots as users play games and do stuff
    - mongo!
- shardable data model for horizontal scalability
- properties are usernames

```
{user1: {games_played: [ordered,
                        list,
                        of,
                        game,
                        ids],
         date_joined: "some date standard",
         pass_hash:,
         other_things...},
 user2: ...}
 ```

Recorded Games

### Transient (Per Server)

Session Store

Active Games

- used to manage websocket gamerooms
