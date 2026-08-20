---
applyTo: "**/*.sql"
---
# SQL Server Instructions

- Write T-SQL for Microsoft SQL Server unless the project clearly targets another engine.
- Use schema-qualified object names.
- Avoid SELECT *.
- Parameterize values supplied by applications.
- Prefer set-based operations over cursors and row-by-row loops.
- Use transactions for atomic multi-step data changes.
- Include rollback-safe error handling for administrative scripts.
- Consider index impact before adding computed filters, functions, or wide sorts.
- Avoid implicit conversions on indexed columns.
- Use appropriate decimal precision/scale for financial values.
- Preserve PK, FK, unique, check, default, identity, computed columns, triggers, and indexes during migrations.
