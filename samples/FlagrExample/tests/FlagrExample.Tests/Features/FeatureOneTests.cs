using FlagrExample.Features.Impl;

namespace FlagrExample.Tests.Features;

public class FeatureOneTests
{
    [Fact]
    public void Should_WriteFeatureOneMessage_When_Executed()
    {
        // Given
        var feature = new FeatureOne();
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
        writer.ToString().ShouldContain("You choose the Feature One");
    }
}
