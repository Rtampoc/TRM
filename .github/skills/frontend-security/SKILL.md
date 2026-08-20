---
name: frontend-security
description: Use for reviewing frontend security including XSS, DOM injection, token handling, CSRF assumptions, unsafe URLs, third-party scripts, and sensitive browser storage.
---

# Frontend Security

- Treat browser input and API data as untrusted.
- Avoid unsafe HTML insertion.
- Do not store secrets in frontend code.
- Avoid exposing sensitive tokens in logs or URLs.
- Validate redirect/return URLs.
- Understand cookie versus bearer-token CSRF implications.
- Use server-side authorization regardless of hidden UI controls.
- Review third-party scripts and dependencies carefully.

