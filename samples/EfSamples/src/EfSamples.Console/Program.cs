#pragma warning disable S6664
using EfSamples.Console;
using EfSamples.Console.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var services = new ServiceCollection()
    .AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddSimpleConsole(o =>
        {
            o.SingleLine = true;
            o.TimestampFormat = "HH:mm:ss.fff ";
            o.UseUtcTimestamp = false;
            o.IncludeScopes = false;
        });

        // Mostra apenas o que interessa do EF Core no console
        builder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information); // SQL executada
        builder.AddFilter(DbLoggerCategory.Database.Transaction.Name, LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Information);
    })
    .AddDbContext<BloggingContext>((sp, options) =>
    {
        // Se o provider é definido dentro do OnConfiguring, não precisamos repetir aqui.
        // Só anexamos o LoggerFactory e habilitamos opções de diagnóstico.
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
        options
            .UseLoggerFactory(loggerFactory)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()

            // Loga somente eventos de comando do relacional (a SQL) e erros
            .LogTo(
                message => loggerFactory.CreateLogger("EF.SQL").LogInformation(message),
                (eventId, level) =>
                    level >= LogLevel.Information &&
                    (eventId.Name is "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted"
                     || eventId.Name is "Microsoft.EntityFrameworkCore.Database.Command.CommandError"),
                DbContextLoggerOptions.DefaultWithLocalTime);
    })
    .BuildServiceProvider();

await using var scope = services.CreateAsyncScope();
var provider = scope.ServiceProvider;
var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("App");

await using var db = provider.GetRequiredService<BloggingContext>();

Console.Clear();
logger.LogInformation("Database path: {path}", db.DbPath);

// Insert
logger.LogInformation("Inserting a new blog");
db.Add(new Blog { Url = "https://blogodft.com" });
await db.SaveChangesAsync();

// Query
logger.LogInformation("Querying for a blog");
var blog = await db.Blogs
    .OrderBy(b => b.BlogId)
    .FirstAsync();

// Update
logger.LogInformation("Updating the blog and adding a post");
blog.Url = "https://blogdoft.com.br";
blog.Posts.Add(new Post { Title = "Hello world", Content = "I wrote an app using EF Core!" });
await db.SaveChangesAsync();

// Delete
logger.LogInformation("Delete the blog");
db.Remove(blog);
await db.SaveChangesAsync();

logger.LogInformation("Initialize Query data.");

var showResult = (IEnumerable<SamplesTable> records) =>
{
    foreach (var record in records)
    {
        logger.LogInformation(record.ToString());
    }
};

var executeQuery = async (SamplesTableQuery query) =>
{
    logger.LogInformation("Query => {query}", query);
    logger.LogInformation("Predicate => {predicate}", query.ToPredicate());
    var result = await db.SampleTables.Where(query.ToPredicate()).ToListAsync();
    showResult(result);
};

var samples = new List<SamplesTable>
{
    new()
    {
        Id = 1,
        TextualType = "Abcde",
        DecimalType = 100,
        BooleanType = true,
        DateTimeType = DateTime.Now.AddDays(-30),
    },
    new()
    {
        Id = 2,
        TextualType = "Fghijk",
        DecimalType = 200,
        BooleanType = false,
        DateTimeType = DateTime.Now.AddDays(-15),
    },
    new()
    {
        Id = 3,
        TextualType = "Lmnopq",
        DecimalType = 300,
        BooleanType = false,
        DateTimeType = DateTime.Now,
    },
    new()
    {
        Id = 4,
        TextualType = "Rstuv",
        DecimalType = 400,
        BooleanType = false,
        DateTimeType = DateTime.Now.AddDays(15),
    },
    new()
    {
        Id = 5,
        TextualType = "Xwyz",
        DecimalType = 500,
        BooleanType = false,
        DateTimeType = DateTime.Now.AddDays(30),
    },
};

await db.SampleTables.AddRangeAsync(samples);
await db.SaveChangesAsync();

logger.LogInformation("Textual Query");
logger.LogInformation("     Wild card at end.");
var wildAtEnd = new SamplesTableQuery()
{
    TextualType = "A%",
};
await executeQuery(wildAtEnd);

logger.LogInformation("     Wild card at start.");
var wildAtStart = new SamplesTableQuery()
{
    TextualType = "%e",
};
await executeQuery(wildAtStart);

logger.LogInformation("     Wild card in the middle.");
var wildInTheMiddle = new SamplesTableQuery()
{
    TextualType = "%b%",
};
await executeQuery(wildInTheMiddle);

logger.LogInformation("Numeric Query");

logger.LogInformation("     Equality");
await executeQuery(new SamplesTableQuery()
{
    Id = 2,
});


logger.LogInformation("     Between");
await executeQuery(new SamplesTableQuery()
{
    GreaterThan = 100,
    LessThan = 500,
});

logger.LogInformation("Boolean");
logger.LogInformation("     false");
await executeQuery(new SamplesTableQuery()
{
    BooleanType = false,
});

logger.LogInformation("     false");
await executeQuery(new SamplesTableQuery()
{
    BooleanType = true,
});

logger.LogInformation("Check order");
await executeQuery(new SamplesTableQuery()
{
    Id = 1,
    TextualType = "%A%",
    BooleanType = false,
});


db.SampleTables.RemoveRange(samples);
await db.SaveChangesAsync();

