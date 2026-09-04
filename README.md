# Lending API

A .NET 8 REST API demonstrating **data-driven business rules**, clean domain
separation, and a **config-driven database provider switch** — runs locally with
zero setup, deploys to Azure App Service via GitHub Actions.

> Personal portfolio project. The domain (items, loans, blocking indicators) is a
> generic reimplementation of a common records-management pattern — no employer
> code or data.

## The business rule

Items carry **indicators** (fragile, access-restricted, ...). A requester's
category maps to a *virtual* indicator (a member of the public "carries" the
public-request indicator). A loan is **blocked** when the item's indicators and
the requester's indicator appear together in the `IncompatibleIndicators` table.
The block message comes from the rule row itself.

**Adding a new blocking rule is a database insert — no code change, no redeploy.**

## Architecture

```
 Client (Swagger / curl)
        |
        v
 ASP.NET Core 8 Web API  -- GitHub Actions CI/CD --> Azure App Service
        |
        v
 LoanValidationService (domain layer, data-driven rules)
        |
        v
 EF Core 8 --> Azure SQL  (falls back to InMemory locally - zero setup)
```

## Run locally

```bash
dotnet run --project src/LendingApi
# Swagger UI at https://localhost:7201/swagger
```

No database needed — with an empty connection string the API uses EF Core
InMemory and seeds demo data.

### Try it

```bash
# Approved: staff borrowing a restricted item
curl -X POST https://localhost:7201/api/loans \
  -H "Content-Type: application/json" \
  -d '{"itemId":1,"requesterCategory":1,"requesterName":"Staff member"}'

# Blocked: member of the public requesting the same item (422 + reasons)
curl -X POST https://localhost:7201/api/loans \
  -H "Content-Type: application/json" \
  -d '{"itemId":1,"requesterCategory":2,"requesterName":"Public requester"}'
```

## Deploy

1. Create an **Azure App Service** (Free F1 tier, .NET 8) and optionally an
   **Azure SQL** database (free tier available).
2. In App Service → Configuration, set connection string `LendingDb` to the
   Azure SQL connection string (leave unset to run InMemory).
3. Download the App Service **publish profile** and add it as the GitHub secret
   `AZURE_WEBAPP_PUBLISH_PROFILE`.
4. Update `AZURE_WEBAPP_NAME` in `.github/workflows/deploy.yml`.
5. Push to `main` — the workflow builds, publishes, and deploys.

## Roadmap

- [ ] EF Core migrations checked in (`dotnet ef migrations add Initial`)
- [ ] Integration tests (WebApplicationFactory + xUnit)
- [ ] Azure Function (timer): expire loan requests pending > 30 days
- [ ] Managed Identity for SQL auth (replace connection-string secrets)
- [ ] AI-assisted request triage: LLM-suggested access category from item description (advisory-only, human decision gate)
