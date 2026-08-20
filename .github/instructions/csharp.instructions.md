---
applyTo: "**/*.cs"
---
# C# Instructions

- Prefer modern, idiomatic C# that remains compatible with the target framework.
- Use async/await for I/O operations and propagate CancellationToken where appropriate.
- Avoid async void except UI/event handlers.
- Use ConfigureAwait only when the project conventions require it.
- Prefer LINQ that remains readable and translates efficiently when used with EF Core.
- Avoid repeated enumeration of expensive IEnumerable sources.
- Avoid hidden side effects in LINQ expressions.
- Use pattern matching when it improves clarity.
- Respect nullable annotations and avoid null-forgiving operators unless justified.
- Keep exception handling narrow and meaningful.
- Prefer immutable state where it reduces bugs.
- Follow existing naming conventions and project analyzers.
