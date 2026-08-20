---
name: security-review
description: Use for security analysis of .NET, ASP.NET Core, APIs, SQL, file handling, authentication, authorization, secrets, and web input.
---

# Security Review

Check OWASP-relevant issues and .NET-specific implementation mistakes.

Always inspect:
- Authentication.
- Authorization at object/resource level.
- Input validation.
- SQL parameterization.
- Output encoding.
- CSRF protections.
- File upload/path validation.
- Secret handling.
- Logging.
- Error disclosure.
- Dependency trust boundaries.

Treat client-side checks as convenience only; security validation must exist server-side.

