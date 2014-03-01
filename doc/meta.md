#### About all documents

From top to bottom, across all subsections: high level, general requirements
progressing to low level, specific requirements.  In essence, if you see
something about which you need more detail, look lower in the document.

### Development Model

Incremental, using vertical slice prototyping.

Each vertical slice follows a waterfall development model.  Current phase slice must be specified completely and carefully, 
    - Vision
    - Constraints and high level requirements
    - Implementation notes
    - Data models
    - Code

All implementation outside of the current slice may be

- ugly
- hard-coded
- hacky

but it must obey the contractual requirements of the current slice.  In the requirement docs, the slice-specific notes are tagged. Ex: VSLICE_0

At the "end" of a phase, all docs and implementation are copied to a slice-tagged directory for reference, and the slice code and clean requirements are pushed to the current development directory.

Hopefully, eventually, all the slices are complete:)

## Current Phase

### VSLICE_0: Game Communications Engine

- comprehensive comms and processing logging is necessary
    - not a redundantly stored, 10K docs per second, multi machine client ...
    - just a couchdb db with a document storing game ids
- a deterministic game engine is necessary
    - not a linked-list dropping, str-conservative, buffered-movement-vector ...
    - must obey command, hashing, etc. requirements of VSLICE_0
- multiple clients necessary, so users are necessary
    - not password authentication
- an initial game state or two are necessary
    - not a game setup application

## Past Phases

the future

- it's promising
