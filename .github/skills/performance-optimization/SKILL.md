---
name: performance-optimization
description: Use when profiling or optimizing .NET, ASP.NET Core, WinForms, EF Core, SQL Server, memory, CPU, I/O, or large-data workflows.
---

# Performance Optimization

Measure before optimizing.

Check:
- Database round trips.
- Query shape and indexes.
- N+1 patterns.
- Excessive allocations.
- Blocking I/O.
- Repeated serialization.
- Large object graphs.
- UI rendering loops.
- Unbounded in-memory processing.

Prefer fixes that reduce work rather than micro-optimizing syntax.

