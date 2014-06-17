# stateHash

single page client-server application using a seeded random object generator and socket.io to test javascript hashcode libraries.

from the server:

1. when that page is served to the client, it handshakes with the server's socket.io instance.
2. when there are >=1 clients connected to the server, the server broadcasts `generate (<object size>, <object seed>)` commands as soon as there are no pending client responses.
3. to account for slow/misbehaving clients, `generate` commands are broadcast at least every 5 seconds.

from the client:

1. when a client receives a `generate (<size>, <seed>)` command, it uses the command parameters to generate a random object.
2. the object is hashed and serialized.
3. all 3 operations are separately timed.
4. the hash, serialization, and timing data are sent back to the server as a `result (<size>, <seed>, <data>)` command.
