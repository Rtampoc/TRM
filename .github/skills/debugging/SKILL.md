---
name: debugging
description: Use for troubleshooting errors, exceptions, wrong results, build failures, Visual Studio issues, runtime failures, IIS problems, and hard-to-reproduce bugs.
---

# Debugging

Do not change code until the failure is understood enough to form a hypothesis.

## Process
1. Capture exact error text and stack trace.
2. Identify first relevant application frame.
3. Inspect input/state at that point.
4. Trace where the bad value/state originated.
5. Test the hypothesis.
6. Apply the smallest fix.
7. Check similar call paths.

Prefer evidence from logs, debugger values, SQL traces, network responses, and reproducible steps.

