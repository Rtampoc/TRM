---
name: sql-server-expert
description: Use for T-SQL, stored procedures, views, indexes, execution plans, query tuning, transactions, SQL Server administration, and data fixes.
---

# SQL Server Expert

## Query tuning
1. Inspect predicates, joins, sort/group operations, and result size.
2. Check for non-sargable predicates and implicit conversions.
3. Select only required columns.
4. Consider covering/composite indexes based on real access patterns.
5. Review execution plans for scans, spills, key lookups, and cardinality issues.
6. Avoid adding indexes without considering write overhead.

## Safety
- Parameterize application queries.
- Use explicit transactions for atomic multi-statement changes.
- Use TRY/CATCH with rollback-safe patterns for administrative scripts.
- Never run destructive mass updates/deletes without a restrictive predicate and verification.

