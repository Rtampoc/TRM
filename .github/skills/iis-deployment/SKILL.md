---
name: iis-deployment
description: Use for deploying ASP.NET Core or .NET applications to IIS, web.config, app pools, hosting bundle, permissions, environment variables, and deployment troubleshooting.
---

# IIS Deployment

Check:
- Correct .NET Hosting Bundle/runtime installed.
- App pool configuration.
- In-process vs out-of-process hosting.
- `web.config`.
- Site bindings.
- Folder permissions.
- Environment variables.
- Connection strings.
- SQL Server connectivity.
- stdout/logging setup for startup failures.

For HTTP 500.x startup errors, inspect Windows Event Viewer and ASP.NET Core Module logs before guessing.

