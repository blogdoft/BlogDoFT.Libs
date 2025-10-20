#pragma warning disable CS8618
namespace EfSamples.Console.Models;

public class Post
{
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public int BlogId { get; set; }

    public Blog Blog { get; set; }
}
#pragma warning restore CS8618
