namespace BlogDoFT.Libs.Flagr;

internal class EvaluationRequest
{
    public EvaluationRequest()
    {
        FlagTags = new List<string>();
        EnableDebug = false;
    }

    public string? EntityId { get; set; }

    public string? EntityType { get; set; }

    public object? EntityContext { get; set; }

    public bool EnableDebug { get; set; }

    public string? FlagKey { get; set; }

    public List<string> FlagTags { get; }
}
