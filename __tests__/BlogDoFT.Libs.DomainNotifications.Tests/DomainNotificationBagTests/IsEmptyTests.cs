namespace BlogDoFT.Libs.DomainNotifications.Tests.DomainNotificationBagTests;

public class IsEmptyTests : DomainNotificationBagBaseTests
{
    [Fact]
    public void Should_ReturnTrue_When_HasNotification()
    {
        // Given
        var notificationStub1 = BuildNotificationStubs();
        var notificationStub2 = BuildNotificationStubs();
        var notifications = BuildNotification();
        notifications.Add(notificationStub1);
        notifications.Add(notificationStub2);

        // When
        var isEmpty = notifications.IsEmpty();

        // Then
        isEmpty.ShouldBeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_ThereIsNoNotification()
    {
        // Given
        var notifications = BuildNotification();

        // When
        var isEmpty = notifications.IsEmpty();

        // Then
        isEmpty.ShouldBeTrue();
    }
}
