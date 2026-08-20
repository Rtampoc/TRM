---
name: database-design
description: Use for relational schema design, normalization, keys, constraints, relationships, naming, audit fields, and SQL Server data modeling.
---

# Database Design

Design for correctness first.

Use:
- Primary keys for every core entity.
- Foreign keys for relationships.
- Unique constraints for business uniqueness.
- Check constraints for domain rules that belong in the database.
- Appropriate nullability.
- Correct lengths and numeric precision.
- Audit timestamps where required.

Avoid:
- Storing comma-separated relational values.
- Duplicate facts across tables without a clear reason.
- Generic catch-all columns.
- Overusing nullable columns in core domain tables.

