#pragma warning disable CS8618
namespace EfSamples.Console.Models;

public class Blog
{
    public int BlogId { get; set; }

    public string Url { get; set; }

    public List<Post> Posts { get; } = [];
}
#pragma warning restore CS8618
