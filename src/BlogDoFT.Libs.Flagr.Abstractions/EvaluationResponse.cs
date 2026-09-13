namespace BlogDoFT.Libs.Flagr.Abstractions;

public class EvaluationResponse
{
    public EvaluationResponse()
    {
        EvalContext = new EvalContext();
    }

    public int FlagId { get; set; }
    public string FlagKey { get; set; }
    public int FlagSnapshotID { get; set; }
    public int SegmentID { get; set; }
    public int VariantID { get; set; }
    public string VariantKey { get; set; }
    public object VariantAttachment { get; set; }
    public virtual EvalContext EvalContext { get; protected set; }
    public string Timestamp { get; set; }
}
