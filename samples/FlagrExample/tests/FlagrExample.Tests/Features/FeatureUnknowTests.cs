using FlagrExample.Features;
using FlagrExample.Features.Impl;

namespace FlagrExample.Tests.Features;

public class FeatureUnknowTests
{
    [Fact]
    public void Should_WriteWarningAndExecuteDefaultFeature_When_Executed()
    {
        // Given
        var defaultFeature = Substitute.For<IMyFeature>();
        var feature = new FeatureUnknow(defaultFeature);
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        // When
        try
        {
            feature.Execute();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        // Then
        writer.ToString().ShouldContain("Your application has no configured flag!");
        writer.ToString().ShouldContain("Executing default feature:");
        defaultFeature.Received(1).Execute();
    }
}
