---
name: dotnet-csharp-expert
description: Use for implementing, reviewing, or refactoring C# and modern .NET code, including async/await, LINQ, dependency injection, OOP, nullability, and performance.
---

# .NET and C# Expert

## Use this skill when
Working on C#, .NET, LINQ, async code, dependency injection, business logic, services, models, or shared libraries.

## Workflow
1. Determine target framework and language version from the solution.
2. Inspect nearby code for conventions.
3. Implement the smallest maintainable change.
4. Check nullability, exception behavior, disposal, and async flow.
5. Avoid unnecessary allocations or blocking calls in hot paths.
6. Explain any non-obvious language/runtime behavior.

## Rules
- Avoid `.Result` and `.Wait()` in application code.
- Avoid `async void` except event handlers.
- Dispose resources correctly.
- Prefer dependency injection over newing infrastructure dependencies in business classes.
- Keep LINQ readable and avoid repeated enumeration.
- Use `decimal` for monetary calculations unless requirements say otherwise.

