# JOURNEY_LOG.md

## Day 1
- **Date**: 2026-07-24 (Wednesday)
- **Topic**: Encapsulation as invariant protection
- **Estimated Time**: [≤1hr]
- **Actual Time**: 1h
- **Takeaway**: Encapsulation protects object state and enforces invariants.

## Day 2
- **Date**: 2026-07-25 (Thursday)
- **Topic**: Abstraction design (good vs bad interfaces)
- **Estimated Time**: [≤1hr]
- **Actual Time**: 1h
- **Takeaway**: Good abstractions hide implementation details and expose intent.

## Day 3
- **Date**: 2026-07-31 (Friday)
- **Topic**: Inheritance — when to use, when NOT ("is-a" test)
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: Inheritance is adoption, not borrowing — the subclass takes the base's entire public surface and every future addition. Test with behavioural substitutability, not English vocabulary.

## Day 4
- **Date**: 2026-08-01 (Saturday)
- **Topic**: Composition over Inheritance — same feature built both ways
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: Inheritance welds behaviour into the type at compile time; composition makes behaviour a field, so it can be swapped at runtime. Two independent axes of variation grow as `M × N` with inheritance but `M + N` with composition — and inheritance still wins when the base's job is to *enforce a rule*, not to hand out behaviour.
