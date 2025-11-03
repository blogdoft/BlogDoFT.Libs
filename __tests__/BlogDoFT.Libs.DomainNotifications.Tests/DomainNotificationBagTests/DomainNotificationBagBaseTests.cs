namespace BlogDoFT.Libs.DomainNotifications.Tests.DomainNotificationBagTests;

public class DomainNotificationBagBaseTests
{
    protected Faker Faker { get; } = BogusFixtures.Get();

    protected static DomainNotificationBag BuildNotification() =>
        new DomainNotificationBag();

    protected DomainNotification BuildNotificationStubs() =>
        BuildNotificationStubs(1)[0];

    protected List<DomainNotification> BuildNotificationStubs(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(_ => new DomainNotification(Faker.Random.Words(5))
            {
                Code = Faker.Random.Words(),
            })];
}
