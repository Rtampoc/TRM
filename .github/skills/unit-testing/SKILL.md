---
name: unit-testing
description: Use for xUnit, NUnit, MSTest, mocking, integration tests, test design, regression tests, and improving testability.
---

# Unit Testing

Test observable behavior.

Prioritize:
- Business rules.
- Calculations.
- Validation.
- Mapping/transformation.
- Authorization rules.
- Failure behavior.
- Regression cases.

Avoid tests that merely duplicate implementation details.
Use integration tests when the risk lies in EF Core mappings, SQL behavior, routing, middleware, or real serialization.

