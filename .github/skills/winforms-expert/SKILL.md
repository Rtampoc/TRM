---
name: winforms-expert
description: Use for C# WinForms forms, controls, DataGridView, event handlers, threading, async UI work, layout, scrolling, and desktop application architecture.
---

# WinForms Expert

## Rules
- Do not block the UI thread with database, file, or network work.
- Update controls only from the UI thread.
- Avoid embedding complex business logic directly in event handlers.
- Dispose forms, dialogs, timers, streams, images, and database objects.
- Avoid per-row database calls when populating DataGridView.
- Use binding or batch loading for large datasets.
- Preserve DPI scaling and resizing behavior.
- For dynamic panels, verify AutoScroll, Dock, Anchor, AutoSize, and minimum sizes.

