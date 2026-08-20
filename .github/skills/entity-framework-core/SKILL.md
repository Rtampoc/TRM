---
name: entity-framework-core
description: Use for EF Core DbContext, entities, relationships, migrations, LINQ-to-SQL translation, transactions, concurrency, and query performance.
---

# Entity Framework Core

## Query checklist
- Keep filtering, sorting, projection, and pagination server-side.
- Use `AsNoTracking()` for read-only queries.
- Avoid N+1 queries.
- Prefer projection to DTOs over loading wide entity graphs.
- Do not materialize early.
- Verify that methods used inside queries translate to SQL.
- For large datasets, use stable ordering before Skip/Take.

## Updates
- Define transaction boundaries deliberately.
- Handle concurrency when multiple users can edit the same record.
- Avoid blindly calling Update on large detached graphs.
- Validate migration impact before applying schema changes.

