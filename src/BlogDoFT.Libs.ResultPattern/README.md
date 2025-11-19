# BlogDoFT.Libs.ResultPattern

Implementation of a Result<T> / Failure pattern for modeling operation results without exceptions. Includes helpers to create, combine and transform results.

Core concepts

- `Result` and `Result<T>` represent either Success or Failure
- `Failure` carries an error code and message (and optional metadata)

Creating results

```csharp
using BlogDoFT.Libs.ResultPattern;

Result.Ok();
Result.Fail("NOT_FOUND", "Item not found");

Result<int> r = Result<int>.Ok(42);
if (r.IsSuccess)
	Console.WriteLine(r.Value);

var combined = Result.Combine(r1, r2); // returns first failure or success
```

Error handling patterns

- Prefer returning `Result<T>` from domain/service methods instead of throwing exceptions for flow errors.
- Convert `Failure` to HTTP responses in the API layer (see DomainNotifications.Extensions for integration helpers).
