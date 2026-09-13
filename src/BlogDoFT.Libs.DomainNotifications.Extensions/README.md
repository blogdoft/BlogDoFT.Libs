# BlogDoFT.Libs.DomainNotifications.Extensions

Extension methods and helpers to integrate DomainNotifications with other frameworks and common flows (e.g. mapping notifications to HTTP responses).

Usage example

```csharp
// In a controller
if (notificationBag.HasNotifications)
{
	return this.ToBadRequest(notificationBag);
}
```

The helper converts `DomainNotificationBag` into a structured JSON payload with codes and messages suitable for clients.

## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
