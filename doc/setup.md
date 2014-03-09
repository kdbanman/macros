#### non-VSLICE TODO

- detail main use case more

- include alternate flows from main use case

## Vision

Application to build game seeds for the game engine.

User chooses number of players and initialization parameters.
User draws the level, the initial bases, and the initial drop zones.

Upon level creation, the seed is committed to a server-side database for any player to use.  The creator can choose to use the level to start a game immediately.

## Use Cases

### Main

1. client is served setup.html
1. player configures a game from config app
1. player commits game seed to server database
    1. server can now serve gameurl with seeded game
