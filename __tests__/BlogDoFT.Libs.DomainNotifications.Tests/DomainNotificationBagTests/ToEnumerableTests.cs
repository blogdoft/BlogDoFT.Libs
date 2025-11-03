namespace BlogDoFT.Libs.DomainNotifications.Tests.DomainNotificationBagTests;

public class ToEnumerableTests : DomainNotificationBagBaseTests
{
    [Fact]
    public void Should_ReturnAllNotifications_When_ReadAsEnumerable()
    {
        // Given
        var notificationStub1 = BuildNotificationStubs();
        var notificationStub2 = BuildNotificationStubs();
        var notifications = BuildNotification();
        notifications.Add(notificationStub1);
        notifications.Add(notificationStub2);

        // When
        var enumerable = notifications.ToEnumerable();

        // Then
        enumerable.ShouldBeAssignableTo<IEnumerable<DomainNotification>>();
        enumerable.ShouldContain(notificationStub1);
        enumerable.ShouldContain(notificationStub2);
    }

    [Fact]
    public void Should_ReturnEmptyEnumerable_When_ThereIsNoRegisteredNotification()
    {
        // Given
        var notifications = BuildNotification();

        // When
        var enumerable = notifications.ToEnumerable();

        // Then
        enumerable.ShouldBeAssignableTo<IEnumerable<DomainNotification>>();
        enumerable.ShouldBeEmpty();
    }
}
