using System.Text.Json.Nodes;

namespace BlogDoFT.Libs.Flagr.Abstractions;

/// <summary>
/// Represents the response returned by Flagr for a flag evaluation request.
/// </summary>
public class EvaluationResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluationResponse"/> class with an
    /// empty <see cref="EvalContext"/>.
    /// </summary>
    public EvaluationResponse()
    {
        EvalContext = new EvalContext();
    }

    /// <summary>
    /// Gets or sets the identifier of the evaluated flag.
    /// </summary>
    /// <value>
    /// 42
    /// </value>
    public int FlagId { get; set; }

    /// <summary>
    /// Gets or sets the key of the evaluated flag.
    /// </summary>
    /// <value>
    /// "new-checkout-flow"
    /// </value>
    public string? FlagKey { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the flag snapshot used for this evaluation.
    /// </summary>
    /// <value>
    /// 7
    /// </value>
    public int FlagSnapshotID { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the segment that matched the evaluation context.
    /// </summary>
    /// <value>
    /// 3
    /// </value>
    public int SegmentID { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the variant returned for this evaluation.
    /// </summary>
    /// <value>
    /// 1
    /// </value>
    public int VariantID { get; set; }

    /// <summary>
    /// Gets or sets the key of the variant returned for this evaluation.
    /// </summary>
    /// <value>
    /// "enabled"
    /// </value>
    public string? VariantKey { get; set; }

    /// <summary>
    /// Flagr always return a JSON object into key-value paris.
    /// </summary>
    /// <value>
    /// {}
    /// </value>
    public JsonObject? VariantAttachment { get; set; }

    /// <summary>
    /// Gets the context that was used to produce this evaluation result.
    /// </summary>
    public virtual EvalContext EvalContext { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp at which the evaluation was performed.
    /// </summary>
    /// <value>
    /// "2026-09-13T12:00:00Z"
    /// </value>
    public string? Timestamp { get; set; }
}
