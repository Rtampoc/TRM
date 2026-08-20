---
applyTo: "**/*.{js,jsx,ts,tsx,html,css,scss,cshtml,razor}"
---
# Frontend Instructions

- Use semantic HTML and preserve accessibility.
- Build mobile-first responsive layouts.
- Keep keyboard navigation and visible focus states.
- Do not rely on color alone for status.
- Escape or encode untrusted content and avoid unsafe HTML injection.
- Handle loading, empty, success, and error states.
- Keep client-side validation synchronized with server rules, but server validation remains authoritative.
- Avoid duplicate event binding and unnecessary DOM work.
- Avoid fixed layouts that overflow smaller screens.
- Optimize images, fonts, network requests, and JavaScript when they materially affect performance.
- Respect the existing UI framework and design system.
- For ASP.NET applications, preserve anti-forgery and authorization requirements.
- For large tables, prefer server-side filtering, sorting, and pagination.
