#### About all documents

From top to bottom, across all subsections: high level, general requirements
progressing to low level, specific requirements.  In essence, if you see
something about which you need more detail, look lower in the document.

### Development Model

Incremental, using vertical slice prototyping.

Each vertical slice follows a waterfall development model.  Current phase slice must be specified completely and carefully with

- Vision
- Constraints, contracts, and high level requirements
- Data models and implementation notes

Implementation within the current slice should be

- documented
- robust
- maintainable

Implementation outside of the current slice may be

- ugly
- hard-coded
- hacky

but it must obey the contractual requirements of the current slice.  In the requirement docs, the slice-specific notes are tagged VSLICE

At the "end" of a phase, all docs and implementation are copied to a slice-tagged directory for reference, and the slice code and clean requirements are pushed to the current development directory.

Hopefully, eventually, all the slices are complete:)

## Past VSLICE Phases

the future

- it's promising
