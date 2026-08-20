---
applyTo: "**/*.{cshtml,razor,cs}"
---
# ASP.NET Core Instructions

- Keep endpoint/controller code thin.
- Validate model input and return correct status codes.
- Use DTOs/view models for boundary contracts.
- Enforce authorization explicitly.
- Avoid exposing stack traces or database details to clients.
- Use anti-forgery protection for cookie-authenticated state changes.
- Prefer server-side pagination for large tables.
- Avoid synchronous database calls in web requests.
- Use dependency injection and strongly typed configuration.
