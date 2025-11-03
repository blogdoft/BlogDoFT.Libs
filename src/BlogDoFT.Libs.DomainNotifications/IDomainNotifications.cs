namespace BlogDoFT.Libs.DomainNotifications;

/// <summary>
/// Implement the Domain notification pattern. <see href="https://martinfowler.com/eaaDev/Notification.html"/> 
/// </summary>
public interface IDomainNotifications
{
    /// <summary>
    /// Add a new Domain notification to collection.
    /// </summary>
    /// <param name="notification">Notification to be added.</param> 
    void Add(DomainNotification notification);

    /// <summary>
    /// Add a new Domain notification to collection
    /// </summary>
    /// <param name="message">Domain notification message</param>
    /// <param name="code">Domain notification code</param>
    void Add(string message, string? code = null);

    /// <summary>
    /// Check if notification collection is empty.
    /// </summary>
    /// <returns>True: there is no Domain Notification. False: one or more notifications added.</returns>
    bool IsEmpty();

    /// <summary>
    /// Get a new IEnumerable instance from Domain Notifications.
    /// </summary>
    /// <returns></returns>
    IEnumerable<DomainNotification> ToEnumerable();

    /// <summary>
    /// Return a DomainNotification at index
    /// </summary>
    DomainNotification this[int index] { get; }

    /// <summary>
    /// Returns how many DomainNotifications has stored.
    /// </summary>
    /// <returns></returns>
    int Count();
}
