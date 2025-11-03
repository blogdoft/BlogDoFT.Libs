namespace BlogDoFT.Libs.DomainNotifications;

public record DomainNotification
{
    public DomainNotification(string message, string? code = null)
    {
        Message = message;
        Code = code;
    }

    public string? Code { get; init; }

    public string Message { get; }
}
