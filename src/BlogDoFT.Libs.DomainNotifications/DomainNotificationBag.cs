namespace BlogDoFT.Libs.DomainNotifications;

internal class DomainNotificationBag : IDomainNotifications
{
    private readonly List<DomainNotification> _notifications = [];

    public void Add(DomainNotification notification)
        => _notifications.Add(notification);

    public void Add(string message, string? code = null)
    {
        var notification = new DomainNotification(message)
        {
            Code = code,
        };

        Add(notification);
    }

    public DomainNotification this[int index] => _notifications[index];

    public int Count() => _notifications.Count;

    public bool IsEmpty() => _notifications.Count == 0;

    public IEnumerable<DomainNotification> ToEnumerable()
        => _notifications.AsEnumerable();
}
