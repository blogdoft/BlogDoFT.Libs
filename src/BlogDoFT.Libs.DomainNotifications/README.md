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

