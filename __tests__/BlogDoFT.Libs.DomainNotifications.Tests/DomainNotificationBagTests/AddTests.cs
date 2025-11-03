namespace BlogDoFT.Libs.DomainNotifications.Tests.DomainNotificationBagTests;

public class AddNotificationTests : DomainNotificationBagBaseTests
{
    [Fact]
    public void Should_AddSingleMessage_When_ManuallyInputData()
    {
        // Given
        var expectedMessage = Faker.Random.Words(5);
        var expectedCode = Faker.Random.Words();
        var notifications = BuildNotification();

        // When
        notifications.Add(message: expectedMessage, code: expectedCode);

        // Then
        notifications[0].ShouldBeEquivalentTo(new DomainNotification(expectedMessage)
        {
            Code = expectedCode,
        });
    }

    [Fact]
    public void Should_AddMessage_When_ThereIsADomainNotification()
    {
        // Given
        DomainNotification notification = new DomainNotification(Faker.Random.Words(5))
        {
            Code = Faker.Random.Word(),
        };
        var notifications = BuildNotification();

        // When
        notifications.Add(notification);

        // Then
        notifications.Count().ShouldBe(1);
        notifications[0].ShouldBe(notification);
    }
}
