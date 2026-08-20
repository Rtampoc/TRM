# GitHub Copilot Repository Instructions

You are assisting with a professional C#/.NET codebase. Prefer correctness, maintainability, security, and clarity over cleverness.

## General Engineering Rules

- Preserve existing behavior unless the user explicitly requests a behavior change.
- Follow SOLID principles where they improve maintainability; avoid unnecessary abstraction.
- Prefer small, focused methods and classes with clear responsibilities.
- Use meaningful names that reflect business intent.
- Avoid duplicated logic; extract reusable logic when repetition is significant.
- Do not introduce dependencies unless they provide clear value.
- Respect the solution's existing architecture and coding conventions.
- When requirements are ambiguous, infer from nearby code and existing patterns before proposing changes.
- Never hard-code secrets, passwords, API keys, connection strings, or environment-specific credentials.
- Add concise comments only where intent is not obvious from the code.

## C# / .NET

- Use modern C# features when compatible with the target framework.
- Prefer async/await for I/O-bound operations.
- Avoid async void except event handlers.
- Pass CancellationToken through async application layers where appropriate.
- Dispose IDisposable/IAsyncDisposable resources correctly.
- Prefer dependency injection over service location or static global state.
- Use nullable reference types correctly when enabled.
- Avoid unnecessary allocations in hot paths.
- Use records only when value semantics are appropriate.
- Do not swallow exceptions.
- Catch specific exceptions when recovery or meaningful context can be added.
- Preserve stack traces when rethrowing.
- Prefer structured logging over string concatenation.

## ASP.NET Core

- Keep controllers/endpoints thin; move business rules to application/domain services.
- Validate request models.
- Return appropriate HTTP status codes.
- Use DTOs instead of exposing persistence entities directly.
- Enforce authorization at the endpoint and service boundaries where needed.
- Protect state-changing requests from CSRF where cookie authentication is used.
- Avoid leaking internal exception details to clients.
- Use configuration/environment variables for deployment-specific settings.

## Entity Framework Core

- Prefer server-side filtering, projection, sorting, and pagination.
- Use AsNoTracking for read-only queries where appropriate.
- Avoid N+1 query patterns.
- Use Include only when needed; prefer projection for API/reporting queries.
- Do not call ToList/First/Single too early in query composition.
- Review generated SQL for performance-sensitive queries.
- Use explicit transactions only when multiple operations must succeed atomically.
- Keep migrations focused and reversible when practical.

## SQL Server

- Always use parameterized queries.
- Never build SQL by concatenating untrusted input.
- Select only required columns.
- Avoid SELECT * in application queries.
- Consider indexes for frequently filtered, joined, or sorted columns.
- Check execution plans for slow queries.
- Avoid functions on indexed columns in predicates when it prevents index seeks.
- Use appropriate data types and lengths.
- Preserve referential integrity with proper PK/FK/unique/check constraints.

## WinForms

- Keep UI code focused on presentation and event coordination.
- Move business/data-access logic out of forms when practical.
- Do not block the UI thread with long-running work.
- Marshal UI updates back to the UI thread when using background work.
- Dispose forms, dialogs, streams, timers, and database resources correctly.
- For DataGridView-heavy screens, avoid repeated per-row database calls.

## Security

- Treat all external input as untrusted.
- Check for SQL injection, XSS, CSRF, path traversal, insecure deserialization, authorization bypass, and secret exposure.
- Apply least privilege for database and service accounts.
- Do not log passwords, tokens, connection strings, or sensitive personal data.
- Use cryptographically secure APIs for security-sensitive randomness and hashing.

## Testing

- Add or update tests when changing business rules, parsing, calculations, authorization, or data transformations.
- Prefer deterministic tests.
- Mock only true external boundaries; do not over-mock internal implementation details.
- Include edge cases and failure paths.

## Debugging

When troubleshooting:
1. Identify the exact failing behavior.
2. Inspect errors, logs, stack traces, inputs, and environment assumptions.
3. Determine the root cause before changing code.
4. Make the smallest safe fix.
5. Explain why the fix works.
6. Check for regression risks.

## Code Review

When reviewing code, prioritize:
1. Correctness
2. Security
3. Data integrity
4. Performance
5. Maintainability
6. Style

Be specific: identify the file/method/logic at risk, explain the impact, and suggest a concrete fix.
