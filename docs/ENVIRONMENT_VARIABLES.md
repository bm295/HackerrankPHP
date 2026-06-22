# Environment Variables

The API uses standard ASP.NET Core configuration binding. Environment variables override values in `appsettings.json` and `appsettings.Production.json` by using double underscores (`__`) for nested configuration keys.

## Runtime

| Variable | Required | Example | Description |
| --- | --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Recommended | `Production` | Selects the ASP.NET Core runtime environment and loads `appsettings.{Environment}.json` when present. |
| `ASPNETCORE_URLS` | Deployment-specific | `http://+:8080` | Configures the HTTP URL bindings for the service. |

## Database

| Variable | Required | Example | Description |
| --- | --- | --- | --- |
| `ConnectionStrings__DefaultConnection` | Yes in production | `Data Source=/var/lib/tdining/tdining.db` | Production database connection string. Keep secrets out of committed appsettings files. |

## Application settings

| Variable | Required | Example | Description |
| --- | --- | --- | --- |
| `Application__ProductName` | No | `T Dining` | Display name used for product/service metadata. |
| `Application__SupportEmail` | Recommended | `support@tdining.example` | Support contact surfaced to operators and deployment documentation. |
| `Application__Currency` | Recommended | `VND` | ISO-style currency code used by restaurant operations and reports. |
| `Application__TimeZone` | Recommended | `Asia/Ho_Chi_Minh` | IANA timezone identifier for business dates, reports, and operational defaults. |

## Logging

| Variable | Required | Example | Description |
| --- | --- | --- | --- |
| `Logging__LogLevel__Default` | No | `Information` | Default application logging level. |
| `Logging__LogLevel__Microsoft.AspNetCore` | No | `Warning` | Reduces framework log noise in production. |
| `Logging__LogLevel__Microsoft.EntityFrameworkCore.Database.Command` | No | `Warning` | Avoids verbose SQL command logging by default. |

## Production safety notes

- Do not commit real database passwords, API keys, payment credentials, SMTP credentials, or customer data to any `appsettings*.json` file.
- Prefer secret managers, orchestrator secrets, or host-level environment variables for production credentials.
- Leave `ConnectionStrings:DefaultConnection` empty in committed appsettings files and set `ConnectionStrings__DefaultConnection` at deployment time.
