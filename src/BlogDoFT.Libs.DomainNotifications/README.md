# BlogDoFT.Libs.DomainNotifications

Domain notification pattern implementation. Simple, lightweight types to collect domain validation and business rule notifications during command handling and validation flows.

Basic usage

```csharp
var bag = new DomainNotificationBag();
bag.Add(new DomainNotification("INVALID_EMAIL", "Email address is invalid"));

if (bag.HasNotifications)
{
	// return or log aggregated problems
}
```

Integration with API layer

Use `DomainNotifications.Extensions` to map a notification bag to an HTTP-friendly response (400/422) in controllers or middleware.


## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
