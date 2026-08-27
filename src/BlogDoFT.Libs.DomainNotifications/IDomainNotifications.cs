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
    /// <returns>An enumerable collection of the stored domain notifications.</returns>
    IEnumerable<DomainNotification> ToEnumerable();

    /// <summary>
    /// Return a DomainNotification at index
    /// </summary>
    /// <param name="index">The zero-based index of the notification to retrieve.</param>
    /// <returns>The domain notification at the specified index.</returns>
    DomainNotification this[int index] { get; }

    /// <summary>
    /// Returns how many DomainNotifications has stored.
    /// </summary>
    /// <returns>The number of stored domain notifications.</returns>
    int Count();
}
