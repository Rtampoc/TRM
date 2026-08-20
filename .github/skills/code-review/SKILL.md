---
name: code-review
description: Use for reviewing pull requests or code changes for correctness, security, performance, maintainability, regressions, and architecture.
---

# Code Review

Rank findings:
- Critical: data loss, auth bypass, exploitable security issue.
- High: major correctness or production reliability issue.
- Medium: meaningful performance/maintainability defect.
- Low: minor quality or consistency issue.

For each finding include:
- Location.
- Problem.
- Why it matters.
- Concrete fix.

Do not flood reviews with style comments when functional risks exist.

