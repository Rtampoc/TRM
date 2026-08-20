---
name: logging-observability
description: Use for ILogger, Serilog, structured logging, correlation IDs, metrics, diagnostics, and production troubleshooting.
---

# Logging and Observability

- Use structured properties, not string concatenation.
- Include correlation/request identifiers where useful.
- Log meaningful state transitions and failures.
- Avoid duplicate logging at every layer.
- Do not log secrets or sensitive personal data.
- Use appropriate log levels.
- Preserve exception objects when logging failures.

