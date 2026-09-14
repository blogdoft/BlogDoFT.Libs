namespace BlogDoFT.Libs.Flagr.Abstractions;

/// <summary>
/// Represents the evaluation context returned by Flagr, describing the entity and flag
/// that were used to produce an evaluation result.
/// </summary>
public class EvalContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvalContext"/> class with an empty
    /// <see cref="FlagTags"/> collection.
    /// </summary>
    public EvalContext()
    {
        FlagTags = [];
    }

    /// <summary>
    /// Gets or sets the identifier of the entity that was evaluated.
    /// </summary>
    /// <value>
    /// "user-123"
    /// </value>
    public string? EntityID { get; set; }

    /// <summary>
    /// Gets or sets the type of the entity that was evaluated.
    /// </summary>
    /// <value>
    /// "user"
    /// </value>
    public string? EntityType { get; set; }

    /// <summary>
    /// Gets or sets the arbitrary context object supplied for the evaluation, used by
    /// Flagr's constraints to decide which variant applies.
    /// </summary>
    /// <value>
    /// { "plan": "premium" }
    /// </value>
    public object? EntityContext { get; set; }

    /// <summary>
    /// Gets or sets whether debug information was requested for this evaluation.
    /// </summary>
    /// <value>
    /// "true"
    /// </value>
    public string? EnableDebug { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the flag that was evaluated.
    /// </summary>
    /// <value>
    /// 42
    /// </value>
    public int? FlagID { get; set; }

    /// <summary>
    /// Gets or sets the key of the flag that was evaluated.
    /// </summary>
    /// <value>
    /// "new-checkout-flow"
    /// </value>
    public string? FlagKey { get; set; }

    /// <summary>
    /// Gets the tags associated with the evaluated flag.
    /// </summary>
    /// <value>
    /// ["beta", "checkout"]
    /// </value>
    public List<string> FlagTags { get; }
}
