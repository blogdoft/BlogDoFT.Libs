using Microsoft.EntityFrameworkCore;
using WebApi.Api.Features.Sales.Infrastructure.Models;

namespace WebApi.Api.Features.Sales.Infrastructure;

public class SalesDbContext : DbContext
{
    private readonly string _dbPath;

    public SalesDbContext()
    {
        _dbPath = ":memory:";
    }

    public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = System.IO.Path.Join(path, "sales.db");
    }

    public DbSet<SaleModel> Sales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"DataSource={_dbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
