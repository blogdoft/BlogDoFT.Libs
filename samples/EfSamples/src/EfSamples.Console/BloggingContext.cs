using EfSamples.Console.Models;
using Microsoft.EntityFrameworkCore;

namespace EfSamples.Console;

public class BloggingContext : DbContext
{
    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }

    public BloggingContext(DbContextOptions<BloggingContext> options)
        : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }

    public string DbPath { get; }

    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<SamplesTable> SampleTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseSqlite($"Data Source={DbPath}");
        optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=sample;Username=postgres;Password=123");
    }
}
