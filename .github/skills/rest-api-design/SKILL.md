---
name: rest-api-design
description: Use for designing or reviewing REST APIs, routes, DTOs, validation, pagination, filtering, error contracts, status codes, and API versioning.
---

# REST API Design

- Model resources and actions clearly.
- Use nouns for resource routes.
- Use appropriate HTTP verbs and status codes.
- Validate request DTOs.
- Return stable error contracts.
- Paginate large collections.
- Support filtering/sorting explicitly.
- Avoid exposing internal DB IDs if they create authorization risks.
- Enforce resource-level authorization.
- Make retry-sensitive operations idempotent where practical.

