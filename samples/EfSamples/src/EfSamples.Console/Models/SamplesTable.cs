using System.Text.Json;

namespace EfSamples.Console.Models;

public class SamplesTable
{
    public int Id { get; set; }

    public string? TextualType { get; set; }

    public decimal? DecimalType { get; set; }

    public bool? BooleanType { get; set; }

    public DateTime DateTimeType { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
    }
}
