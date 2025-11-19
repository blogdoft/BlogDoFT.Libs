# BlogDoFT.Libs.DapperUtils.Postgres

Dapper utilities and helpers for Postgres. Implements abstractions for pagination, SQL builders and a connection factory tailored to Npgsql.

This package provides concrete implementations for the abstractions defined in `BlogDoFT.Libs.DapperUtils.Abstractions`, and includes:

- Paginated SQL builders and pagination helpers
- OrderBy and Where clause helpers
- `IConnectionFactory` implementation that reads connection string from `IConfiguration`

Quick start

1. Reference both `BlogDoFT.Libs.DapperUtils.Abstractions` and `BlogDoFT.Libs.DapperUtils.Postgres` from your data project.
2. Register the connection factory in DI and use the `IConnectionFactory` to get `IDbConnection` instances:

```csharp
// Program.cs
builder.Services.AddSingleton<IConnectionFactory>(sp =>
		new NpgConnectionFactory(sp.GetRequiredService<IConfiguration>()));

// Usage
using var conn = serviceProvider.GetRequiredService<IConnectionFactory>().GetNewConnection();
var rows = await conn.QueryAsync<MyDto>("SELECT * FROM my_table LIMIT 10");
```

Configuration (appsettings.json)

This library expects a connection string named `Default` in the `ConnectionStrings` section. Example:

```json
{
	"ConnectionStrings": {
		"Default": "Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=secret"
	}
}
```

Notes and tips
- The Npgsql connection factory sets the `SearchPath` to `public` by default. If your schema differs, update the connection builder in a custom factory or pass schema via configuration.
- The project registers Dapper type handlers for `DateOnly` and `TimeOnly` to map to Postgres types. Ensure your database uses compatible column types.
