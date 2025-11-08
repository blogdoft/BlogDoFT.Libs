using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.DomainNotifications.Tests;

public sealed class DomainNotificationExtensionTests
{
    private readonly Faker _faker = BogusFixtures.Get();

    [Fact]
    public void Should_RegisterScopedDomainNotificationBag_When_AddDomainNotification()
    {
        // Given
        var services = new ServiceCollection();
        var expectedMessage = _faker.Random.Words(3);
        var expectedCode = _faker.Random.AlphaNumeric(6);

        // When
        services.AddDomainNotification();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var notifications = scope.ServiceProvider.GetRequiredService<IDomainNotifications>();
        notifications.Add(expectedMessage, expectedCode);

        // Then
        notifications.ShouldBeOfType<DomainNotificationBag>();
        notifications.IsEmpty().ShouldBeFalse();
        notifications[0].Message.ShouldBe(expectedMessage);
        notifications[0].Code.ShouldBe(expectedCode);
    }

    [Fact]
    public void Should_AddScopedDescriptor_When_RegisteringDomainNotifications()
    {
        // Given
        var services = Substitute.For<IServiceCollection>();
        ServiceDescriptor? descriptor = null;
        services.When(s => s.Add(Arg.Any<ServiceDescriptor>()))
            .Do(call => descriptor = call.Arg<ServiceDescriptor>());

        // When
        var returnedServices = DomainNotificationExtension.AddDomainNotification(services);

        // Then
        returnedServices.ShouldBe(services);
        descriptor.ShouldNotBeNull();
        descriptor!.ServiceType.ShouldBe(typeof(IDomainNotifications));
        descriptor.ImplementationType.ShouldBe(typeof(DomainNotificationBag));
        descriptor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }
}
