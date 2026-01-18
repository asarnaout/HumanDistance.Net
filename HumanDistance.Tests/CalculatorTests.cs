namespace HumanDistance.Tests;

public class CalculatorTests
{
    [Fact]
    public void Calculate_IdenticalStrings_ReturnsZero()
    {
        var result = Calculator.Calculate("hello", "hello");
        Assert.Equal(0, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(0, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
        Assert.Equal(0.0, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_EmptyStrings_ReturnsZero()
    {
        var result = Calculator.Calculate("", "");
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_EmptySource_ReturnsTargetLength()
    {
        var result = Calculator.Calculate("", "hello");
        Assert.Equal(5, result.EditDistance);
        Assert.Equal(5, result.Insertions);
        Assert.Equal(0, result.Deletions);
    }

    [Fact]
    public void Calculate_EmptyTarget_ReturnsSourceLength()
    {
        var result = Calculator.Calculate("hello", "");
        Assert.Equal(5, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(5, result.Deletions);
    }

    [Fact]
    public void Calculate_SingleInsertion_ReturnsOne()
    {
        var result = Calculator.Calculate("hell", "hello");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Insertions);
        Assert.Equal(0, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
    }

    [Fact]
    public void Calculate_SingleDeletion_ReturnsOne()
    {
        var result = Calculator.Calculate("hello", "hell");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(1, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
    }

    [Fact]
    public void Calculate_SingleSubstitution_ReturnsOne()
    {
        var result = Calculator.Calculate("hello", "hallo");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.True(result.KeyboardDistanceSum > 0.0, "Keyboard distance should be positive for substitution");
        Assert.True(result.KeyboardDistanceSum <= 1.0, "Single substitution keyboard distance should be at most 1.0");
    }

    [Fact]
    public void Calculate_Transposition_ReturnsOne()
    {
        var result = Calculator.Calculate("hello", "hlelo");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Transpositions);
        Assert.Equal(0, result.Substitutions);
    }

    [Fact]
    public void Calculate_CaseInsensitive()
    {
        var result = Calculator.Calculate("Hello", "hello");
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_AdjacentKeys_HasLowKeyboardDistance()
    {
        var result = Calculator.Calculate("a", "s");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_DistantKeys_HasHigherKeyboardDistance()
    {
        var adjacentResult = Calculator.Calculate("a", "s");
        var distantResult = Calculator.Calculate("a", "p");
        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_UnknownCharacters_DefaultToFullCost()
    {
        var result = Calculator.Calculate("a", "α");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.Equal(1.0, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_DefaultsToQwerty()
    {
        var result = Calculator.Calculate("q", "a");
        var qwertyResult = Calculator.Calculate("q", "a", KeyboardLayout.Qwerty);
        Assert.Equal(qwertyResult.EditDistance, result.EditDistance);
        Assert.Equal(qwertyResult.KeyboardDistanceSum, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_WithLayout_IdenticalStrings_ReturnsZero()
    {
        var result = Calculator.Calculate("hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_AdjacentKeys_HasLowKeyboardDistance()
    {
        var result = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        Assert.Equal(1, result.EditDistance);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_WithLayout_DistantKeys_HasHigherKeyboardDistance()
    {
        var adjacentResult = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        var distantResult = Calculator.Calculate("a", "p", KeyboardLayout.Qwerty);
        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_WithLayout_QwertyLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("qwerty", "qwerty", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_AzertyLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("azerty", "azerty", KeyboardLayout.Azerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_QwertzLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("qwertz", "qwertz", KeyboardLayout.Qwertz);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_CaseInsensitive()
    {
        var result = Calculator.Calculate("Hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Theory]
    [InlineData("", "", 0, 0, 0)]
    [InlineData("a", "", 1, 0, 1)]
    [InlineData("", "a", 1, 1, 0)]
    [InlineData("abc", "abc", 0, 0, 0)]
    [InlineData("abc", "ab", 1, 0, 1)]
    [InlineData("ab", "abc", 1, 1, 0)]
    [InlineData("abc", "acb", 1, 0, 0)] // Transposition
    public void Calculate_VariousInputs_ReturnsExpectedDistance(
        string source, string target, int expectedDistance, int expectedInsertions, int expectedDeletions)
    {
        var result = Calculator.Calculate(source, target);
        Assert.Equal(expectedDistance, result.EditDistance);
        Assert.Equal(expectedInsertions, result.Insertions);
        Assert.Equal(expectedDeletions, result.Deletions);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwerty, "q", "w", "q", "m")]
    [InlineData(KeyboardLayout.Azerty, "a", "z", "a", "p")]
    [InlineData(KeyboardLayout.Azerty, "q", "s", "q", "m")]
    [InlineData(KeyboardLayout.Qwertz, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwertz, "q", "w", "q", "m")]
    public void Calculate_WithLayout_AdjacentKeysLowerKeyboardDistanceThanDistantKeys(
        KeyboardLayout layout, string source1, string adjacentTarget, string source2, string distantTarget)
    {
        var adjacentResult = Calculator.Calculate(source1, adjacentTarget, layout);
        var distantResult = Calculator.Calculate(source2, distantTarget, layout);
        Assert.True(adjacentResult.KeyboardDistanceSum < distantResult.KeyboardDistanceSum,
            $"Layout {layout}: Adjacent keys ({source1}->{adjacentTarget}={adjacentResult.KeyboardDistanceSum:F3}) should have lower keyboard distance than distant keys ({source2}->{distantTarget}={distantResult.KeyboardDistanceSum:F3})");
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void Calculate_WithLayout_UnknownCharacters_DefaultToFullCost(KeyboardLayout layout)
    {
        var result = Calculator.Calculate("a", "α", layout);
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1.0, result.KeyboardDistanceSum);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "ab", "ba")]
    [InlineData(KeyboardLayout.Qwerty, "qp", "pq")]
    [InlineData(KeyboardLayout.Azerty, "az", "za")]
    [InlineData(KeyboardLayout.Azerty, "qm", "mq")]
    [InlineData(KeyboardLayout.Qwertz, "ab", "ba")]
    [InlineData(KeyboardLayout.Qwertz, "qp", "pq")]
    public void Calculate_WithLayout_Transposition_ReturnsOne(
        KeyboardLayout layout, string source, string target)
    {
        var result = Calculator.Calculate(source, target, layout);
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Transpositions);
        Assert.Equal(0, result.Substitutions);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "", "", 0)]
    [InlineData(KeyboardLayout.Qwerty, "a", "", 1)]
    [InlineData(KeyboardLayout.Qwerty, "", "a", 1)]
    [InlineData(KeyboardLayout.Qwerty, "abc", "abc", 0)]
    [InlineData(KeyboardLayout.Qwerty, "Hello", "hello", 0)]
    [InlineData(KeyboardLayout.Azerty, "", "", 0)]
    [InlineData(KeyboardLayout.Azerty, "a", "", 1)]
    [InlineData(KeyboardLayout.Azerty, "", "a", 1)]
    [InlineData(KeyboardLayout.Azerty, "abc", "abc", 0)]
    [InlineData(KeyboardLayout.Azerty, "Hello", "hello", 0)]
    [InlineData(KeyboardLayout.Qwertz, "", "", 0)]
    [InlineData(KeyboardLayout.Qwertz, "a", "", 1)]
    [InlineData(KeyboardLayout.Qwertz, "", "a", 1)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "abc", 0)]
    [InlineData(KeyboardLayout.Qwertz, "Hello", "hello", 0)]
    public void Calculate_WithLayout_VariousInputs_ReturnsExpectedEditDistance(
        KeyboardLayout layout, string source, string target, int expectedDistance)
    {
        var result = Calculator.Calculate(source, target, layout);
        Assert.Equal(expectedDistance, result.EditDistance);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "abc", "adc", 1, 0.2147205770015717)]
    [InlineData(KeyboardLayout.Qwerty, "a", "z", 1, 0.08915787190198898)]
    [InlineData(KeyboardLayout.Azerty, "abc", "adc", 1, 0.22438186407089233)]
    [InlineData(KeyboardLayout.Azerty, "a", "z", 1, 0.0833333358168602)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "adc", 1, 0.22438186407089233)]
    [InlineData(KeyboardLayout.Qwertz, "a", "z", 1, 0.40451017022132874)]
    public void Calculate_WithLayout_SubstitutionWithExpectedKeyboardDistance(
        KeyboardLayout layout, string source, string target, int expectedSubstitutions, double expectedKeyboardDistance)
    {
        var result = Calculator.Calculate(source, target, layout);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedKeyboardDistance, result.KeyboardDistanceSum, precision: 10);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "abc", "bad", 1, 1, 0.08915787190198898)]
    [InlineData(KeyboardLayout.Azerty, "abc", "bad", 1, 1, 0.0931695029139519)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "bad", 1, 1, 0.0931695029139519)]
    public void Calculate_WithLayout_TranspositionPlusSubstitution(
        KeyboardLayout layout, string source, string target,
        int expectedTranspositions, int expectedSubstitutions, double expectedKeyboardDistance)
    {
        var result = Calculator.Calculate(source, target, layout);
        Assert.Equal(expectedTranspositions, result.Transpositions);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedKeyboardDistance, result.KeyboardDistanceSum, precision: 10);
    }

    [Theory]
    [InlineData("kitten", "sitting", 3, 1, 0, 2, 0)] // s->k sub, i->e sub, g insert
    [InlineData("saturday", "sunday", 3, 0, 2, 1, 0)] // at delete, delete r, n->r sub
    [InlineData("abc", "cba", 2, 0, 0, 2, 0)] // a->c, c->a subs (not transposition since not adjacent)
    [InlineData("ab", "ba", 1, 0, 0, 0, 1)] // Transposition
    [InlineData("test", "toast", 2, 1, 0, 1, 0)] // e->o sub, a insert
    public void Calculate_EditDistanceEqualsOperationSum(
        string source, string target,
        int expectedDistance, int expectedInsertions, int expectedDeletions,
        int expectedSubstitutions, int expectedTranspositions)
    {
        var result = Calculator.Calculate(source, target);
        Assert.Equal(expectedDistance, result.EditDistance);
        Assert.Equal(expectedInsertions, result.Insertions);
        Assert.Equal(expectedDeletions, result.Deletions);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedTranspositions, result.Transpositions);

        // Verify EditDistance equals sum of all operations
        Assert.Equal(result.EditDistance,
            result.Insertions + result.Deletions + result.Substitutions + result.Transpositions);
    }

    [Fact]
    public void Calculate_NoSubstitutions_KeyboardDistanceSumIsZero()
    {
        // Pure insertions
        var insertResult = Calculator.Calculate("abc", "abcde");
        Assert.Equal(0, insertResult.Substitutions);
        Assert.Equal(0.0, insertResult.KeyboardDistanceSum);

        // Pure deletions
        var deleteResult = Calculator.Calculate("hello", "hel");
        Assert.Equal(0, deleteResult.Substitutions);
        Assert.Equal(0.0, deleteResult.KeyboardDistanceSum);

        // Pure transposition
        var transposeResult = Calculator.Calculate("ab", "ba");
        Assert.Equal(0, transposeResult.Substitutions);
        Assert.Equal(0.0, transposeResult.KeyboardDistanceSum);

        // Identical strings
        var identicalResult = Calculator.Calculate("test", "test");
        Assert.Equal(0, identicalResult.Substitutions);
        Assert.Equal(0.0, identicalResult.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_AverageKeyboardDistance_CalculatedCorrectly()
    {
        // Single substitution
        var singleResult = Calculator.Calculate("a", "s");
        Assert.Equal(singleResult.KeyboardDistanceSum, singleResult.AverageKeyboardDistance);

        // Multiple substitutions
        var multiResult = Calculator.Calculate("abc", "xyz");
        Assert.Equal(3, multiResult.Substitutions);
        Assert.Equal(multiResult.KeyboardDistanceSum / 3, multiResult.AverageKeyboardDistance);
    }

    [Fact]
    public void Calculate_NoSubstitutions_AverageKeyboardDistanceIsZero()
    {
        var result = Calculator.Calculate("abc", "abcde"); // Pure insertions
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0.0, result.AverageKeyboardDistance);
    }

    [Theory]
    [InlineData("password", "pasword", 1, 0, 1, 0, 0)]
    [InlineData("password", "Password", 0, 0, 0, 0, 0)] // Case insensitive
    [InlineData("password", "passowrd", 1, 0, 0, 0, 1)] // Transposition
    [InlineData("password", "p4ssword", 1, 0, 0, 1, 0)] // Substitution (a->4)
    public void Calculate_TypicalTypoScenarios(
        string source, string target,
        int expectedDistance, int expectedInsertions, int expectedDeletions,
        int expectedSubstitutions, int expectedTranspositions)
    {
        var result = Calculator.Calculate(source, target);
        Assert.Equal(expectedDistance, result.EditDistance);
        Assert.Equal(expectedInsertions, result.Insertions);
        Assert.Equal(expectedDeletions, result.Deletions);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedTranspositions, result.Transpositions);
    }

    [Fact]
    public void Calculate_MultipleSubstitutions_KeyboardDistanceSumAccumulates()
    {
        // Get individual keyboard distances
        var azResult = Calculator.Calculate("a", "z", KeyboardLayout.Qwerty);
        var bxResult = Calculator.Calculate("b", "x", KeyboardLayout.Qwerty);

        // Calculate combined
        var combinedResult = Calculator.Calculate("ab", "zx", KeyboardLayout.Qwerty);

        Assert.Equal(2, combinedResult.Substitutions);
        Assert.Equal(azResult.KeyboardDistanceSum + bxResult.KeyboardDistanceSum,
            combinedResult.KeyboardDistanceSum, precision: 10);
    }
}
