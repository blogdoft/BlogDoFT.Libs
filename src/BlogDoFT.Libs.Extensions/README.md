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
