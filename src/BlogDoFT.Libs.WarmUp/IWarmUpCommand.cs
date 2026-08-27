namespace BlogDoFT.Libs.WarmUp;

/// <summary>
/// Represents a command that performs application warm-up work, executed as part of the warm-up hosted service.
/// </summary>
public interface IWarmUpCommand
{
    /// <summary>
    /// Executes the warm-up work for this command.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Execute();
}
