namespace HumanDistance;

public readonly struct DistanceResult
{
    /// <summary>
    /// Standard Damerau-Levenshtein edit distance (all operations cost 1).
    /// </summary>
    public int EditDistance { get; init; }

    /// <summary>
    /// Number of insertion operations.
    /// </summary>
    public int Insertions { get; init; }

    /// <summary>
    /// Number of deletion operations.
    /// </summary>
    public int Deletions { get; init; }

    /// <summary>
    /// Number of substitution operations.
    /// </summary>
    public int Substitutions { get; init; }

    /// <summary>
    /// Number of transposition operations.
    /// </summary>
    public int Transpositions { get; init; }

    /// <summary>
    /// Sum of normalized keyboard distances for each substitution.
    /// Each substitution contributes (distance / maxDistance), so range is [0, Substitutions].
    /// </summary>
    public double KeyboardDistanceSum { get; init; }

    /// <summary>
    /// Average normalized keyboard distance per substitution.
    /// Range [0, 1] where 0 = same key, 1 = maximum distance apart.
    /// Returns 0 when there are no substitutions.
    /// </summary>
    public double AverageKeyboardDistance =>
        Substitutions > 0 ? KeyboardDistanceSum / Substitutions : 0.0;

}
