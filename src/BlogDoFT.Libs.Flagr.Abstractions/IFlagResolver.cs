namespace BlogDoFT.Libs.Flagr.Abstractions;

public interface IFlagResolver
{
    EvaluationResponse ResolveFlag<T>(object entityContext);
}
