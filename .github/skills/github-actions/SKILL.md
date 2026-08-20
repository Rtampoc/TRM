---
name: github-actions
description: Use for GitHub Actions CI/CD workflows, build/test pipelines, caching, artifacts, deployment jobs, secrets, and failing workflow diagnostics.
---

# GitHub Actions

- Pin action versions appropriately.
- Use repository/environment secrets instead of hard-coded values.
- Keep permissions least-privileged.
- Cache only when it provides meaningful benefit.
- Separate build/test/deploy concerns.
- Upload useful diagnostics on failure.
- For .NET, restore, build, and test using explicit configuration and project/solution paths.

