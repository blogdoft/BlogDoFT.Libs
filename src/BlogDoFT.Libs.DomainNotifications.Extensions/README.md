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
