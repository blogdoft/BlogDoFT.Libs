# BlogDoFT.Libs.Extensions

General purpose extension methods used across BlogDoFT projects. Contains string helpers, collection helpers and small utilities that improve readability and reduce boilerplate.

Common utilities

- String helpers: null/empty checks, sanitize, safe truncation
- Collection helpers: safe AddRange, ToReadOnly, chunking helpers

Usage example

```csharp
using BlogDoFT.Libs.Extensions;

string? maybe = null;
var safe = maybe.OrEmpty(); // ""

var list = new List<int> {1,2,3};
list.AddRangeSafe(null); // no-op

var chunked = list.ChunkBy(2); // [[1,2],[3]]
```

Notes
Import the namespace `BlogDoFT.Libs.Extensions` where you want the extension methods available.

## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
