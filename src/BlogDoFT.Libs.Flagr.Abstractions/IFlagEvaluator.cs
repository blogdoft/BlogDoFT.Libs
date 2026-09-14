namespace BlogDoFT.Libs.Flagr.Abstractions;

/// <summary>
/// Evaluates feature flags against Flagr, using a type as the flag key.
/// </summary>
public interface IFlagEvaluator
{
    /// <summary>
    /// Evaluates the flag whose key matches the name of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// A type whose name is used as the flag key. For example, a type named
    /// <c>NewCheckoutFlow</c> evaluates the flag with key "NewCheckoutFlow".
    /// </typeparam>
    /// <param name="entityContext">
    /// The entity context used by Flagr's constraints to select a variant, e.g.
    /// <c>{ "userId": "123", "plan": "premium" }</c>.
    /// </param>
    /// <returns>The <see cref="EvaluationResponse"/> returned by Flagr.</returns>
    EvaluationResponse EvaluateFlag<T>(object? entityContext = null);
}
