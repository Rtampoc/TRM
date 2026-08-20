---
name: refactoring
description: Use for restructuring legacy or complex code while preserving behavior, reducing duplication, improving boundaries, and increasing maintainability.
---

# Refactoring

Preserve external behavior unless explicitly asked otherwise.

Sequence:
1. Characterize current behavior.
2. Add tests around risky logic when practical.
3. Refactor in small coherent steps.
4. Keep commits/changes reviewable.
5. Remove duplication and clarify responsibilities.
6. Avoid introducing abstractions with only one speculative use.

