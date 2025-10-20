#pragma warning disable CS8618
using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;
using System.Text.Json;

namespace EfSamples.Console.Models;

[GeneratePredicate<SamplesTable>()]
public partial class SamplesTableQuery
{
    [NumericFilter(Operator = ComparisonOperator.Equal, Order = 1, TargetProperty = nameof(SamplesTable.Id))]
    public int? Id { get; set; }

    [StringFilter(Order = 3, TargetProperty = nameof(SamplesTable.TextualType))]
    public string? TextualType { get; set; }

    [NumericFilter(Operator = ComparisonOperator.GreaterThan, TargetProperty = nameof(SamplesTable.DecimalType))]
    public decimal? GreaterThan { get; set; }

    [NumericFilter(Operator = ComparisonOperator.LessThan, TargetProperty = nameof(SamplesTable.DecimalType))]
    public decimal? LessThan { get; set; }

    [BooleanFilter(Order = 2)]
    public bool? BooleanType { get; set; }

    [TemporalFilter(Operator = ComparisonOperator.Equal)]
    public DateTime? DateTimeType { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
#pragma warning restore CS8618
