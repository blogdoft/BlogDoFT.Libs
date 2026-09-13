namespace BlogDoFT.Libs.Flagr.Abstractions;

public interface IFlagEvaluator
{
    EvaluationResponse EvaluateFlag<T>(object entityContext);
}
