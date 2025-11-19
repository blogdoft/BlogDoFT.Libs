# BlogDoFT.Libs.Api

Lightweight helpers and conventions for building HTTP APIs. This library contains small helpers and extension patterns used across BlogDoFT Web API projects.

This README describes common integration patterns rather than a complete API reference. See the repository for implementation details.

Common patterns

1) Mapping domain `Result<T>` to HTTP responses

```csharp
// Example service method
public Result<UserDto> GetUser(Guid id) { ... }

// In controller action
var result = userService.GetUser(id);
if (result.IsFailure)
		return result.Failure.Code switch
		{
				"NOT_FOUND" => Results.NotFound(new { code = result.Failure.Code, message = result.Failure.Message }),
				_ => Results.BadRequest(new { code = result.Failure.Code, message = result.Failure.Message })
		};

return Results.Ok(result.Value);
```

2) Mapping `DomainNotificationBag` to HTTP responses

```csharp
if (notificationBag.HasNotifications)
{
		var errors = notificationBag.Notifications
				.Select(n => new { code = n.Code, message = n.Message });

		return Results.BadRequest(new { errors });
}
```

3) Middleware wiring suggestions

- Add a small middleware or filter that converts `Result` / `DomainNotificationBag` to HTTP responses centrally to avoid repetitive controller code.
- When returning validation-like problems, prefer 400/422 with a stable problem payload: `{ "errors": [{"code":"...","message":"..."}] }`.

Examples and recommended payloads

```json
{
	"errors": [
		{ "code": "INVALID_EMAIL", "message": "Email is invalid" }
	]
}
```

Notes
- This project is intentionally small — prefer to keep API-layer concerns (presentation, HTTP mapping) decoupled from domain logic. Use `BlogDoFT.Libs.ResultPattern` and `BlogDoFT.Libs.DomainNotifications` to standardize domain results and notifications.
