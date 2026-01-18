namespace HumanDistance.Tests;

public class CalculatorTests
{
    [Fact]
    public void Calculate_IdenticalStrings_ReturnsZero()
    {
        double result = Calculator.Calculate("hello", "hello");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_EmptyStrings_ReturnsZero()
    {
        double result = Calculator.Calculate("", "");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_EmptySource_ReturnsTargetLength()
    {
        double result = Calculator.Calculate("", "hello");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Calculate_EmptyTarget_ReturnsSourceLength()
    {
        double result = Calculator.Calculate("hello", "");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Calculate_SingleInsertion_ReturnsOne()
    {
        double result = Calculator.Calculate("hell", "hello");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Calculate_SingleDeletion_ReturnsOne()
    {
        double result = Calculator.Calculate("hello", "hell");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Calculate_SingleSubstitution_ReturnsAtMostOne()
    {
        double result = Calculator.Calculate("hello", "hallo");
        Assert.True(result <= 1.0, $"Single substitution should cost at most 1.0, got {result}");
        Assert.True(result > 0.0, $"Single substitution should cost more than 0, got {result}");
    }

    [Fact]
    public void Calculate_Transposition_ReturnsOne()
    {
        double result = Calculator.Calculate("hello", "hlelo");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Calculate_CaseInsensitive()
    {
        double result = Calculator.Calculate("Hello", "hello");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_AdjacentKeys_ReturnsLowCost()
    {
        double result = Calculator.Calculate("a", "s");
        Assert.True(result < 0.5, $"Adjacent keys 'a' and 's' should have low cost, got {result}");
    }

    [Fact]
    public void Calculate_DistantKeys_ReturnsHigherCost()
    {
        double adjacentResult = Calculator.Calculate("a", "s");
        double distantResult = Calculator.Calculate("a", "p");
        Assert.True(distantResult > adjacentResult,
            $"Distant keys should have higher cost than adjacent keys. Adjacent: {adjacentResult}, Distant: {distantResult}");
    }

    [Fact]
    public void Calculate_UnknownCharacters_DefaultToFullCost()
    {
        double result = Calculator.Calculate("a", "α");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Calculate_DefaultsToQwerty()
    {
        double result = Calculator.Calculate("q", "a");
        double qwertyResult = Calculator.Calculate("q", "a", KeyboardLayout.Qwerty);
        Assert.Equal(qwertyResult, result);
    }

    [Fact]
    public void Calculate_WithLayout_IdenticalStrings_ReturnsZero()
    {
        double result = Calculator.Calculate("hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithLayout_AdjacentKeys_ReturnsLowCost()
    {
        double result = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        Assert.True(result < 0.5, $"Adjacent keys 'a' and 's' should have low cost, got {result}");
    }

    [Fact]
    public void Calculate_WithLayout_DistantKeys_ReturnsHigherCost()
    {
        double adjacentResult = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        double distantResult = Calculator.Calculate("a", "p", KeyboardLayout.Qwerty);
        Assert.True(distantResult > adjacentResult,
            $"Distant keys should have higher cost than adjacent keys. Adjacent: {adjacentResult}, Distant: {distantResult}");
    }

    [Fact]
    public void Calculate_WithLayout_QwertyLayout_WorksCorrectly()
    {
        double result = Calculator.Calculate("qwerty", "qwerty", KeyboardLayout.Qwerty);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithLayout_AzertyLayout_WorksCorrectly()
    {
        double result = Calculator.Calculate("azerty", "azerty", KeyboardLayout.Azerty);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithLayout_QwertzLayout_WorksCorrectly()
    {
        double result = Calculator.Calculate("qwertz", "qwertz", KeyboardLayout.Qwertz);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithLayout_CaseInsensitive()
    {
        double result = Calculator.Calculate("Hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0.0, result);
    }

    [Theory]
    [InlineData("", "", 0.0)]
    [InlineData("a", "", 1.0)]
    [InlineData("", "a", 1.0)]
    [InlineData("abc", "abc", 0.0)]
    [InlineData("abc", "ab", 1.0)]
    [InlineData("ab", "abc", 1.0)]
    [InlineData("abc", "acb", 1.0)]
    public void Calculate_VariousInputs_ReturnsExpectedDistance(string source, string target, double expected)
    {
        double result = Calculator.Calculate(source, target);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "", "", 0.0)]
    [InlineData(KeyboardLayout.Qwerty, "a", "", 1.0)]
    [InlineData(KeyboardLayout.Qwerty, "", "a", 1.0)]
    [InlineData(KeyboardLayout.Qwerty, "abc", "abc", 0.0)]
    [InlineData(KeyboardLayout.Qwerty, "Hello", "hello", 0.0)]
    [InlineData(KeyboardLayout.Azerty, "", "", 0.0)]
    [InlineData(KeyboardLayout.Azerty, "a", "", 1.0)]
    [InlineData(KeyboardLayout.Azerty, "", "a", 1.0)]
    [InlineData(KeyboardLayout.Azerty, "abc", "abc", 0.0)]
    [InlineData(KeyboardLayout.Azerty, "Hello", "hello", 0.0)]
    [InlineData(KeyboardLayout.Qwertz, "", "", 0.0)]
    [InlineData(KeyboardLayout.Qwertz, "a", "", 1.0)]
    [InlineData(KeyboardLayout.Qwertz, "", "a", 1.0)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "abc", 0.0)]
    [InlineData(KeyboardLayout.Qwertz, "Hello", "hello", 0.0)]
    public void Calculate_WithLayout_VariousInputs_ReturnsExpectedDistance(
        KeyboardLayout layout, string source, string target, double expected)
    {
        double result = Calculator.Calculate(source, target, layout);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwerty, "q", "w", "q", "m")]
    [InlineData(KeyboardLayout.Azerty, "a", "z", "a", "p")]
    [InlineData(KeyboardLayout.Azerty, "q", "s", "q", "m")]
    [InlineData(KeyboardLayout.Qwertz, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwertz, "q", "w", "q", "m")]
    public void Calculate_WithLayout_AdjacentKeysLowerCostThanDistantKeys(
        KeyboardLayout layout, string source1, string adjacentTarget, string source2, string distantTarget)
    {
        double adjacentResult = Calculator.Calculate(source1, adjacentTarget, layout);
        double distantResult = Calculator.Calculate(source2, distantTarget, layout);
        Assert.True(adjacentResult < distantResult,
            $"Layout {layout}: Adjacent keys ({source1}->{adjacentTarget}={adjacentResult:F3}) should have lower cost than distant keys ({source2}->{distantTarget}={distantResult:F3})");
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void Calculate_WithLayout_UnknownCharacters_DefaultToFullCost(KeyboardLayout layout)
    {
        double result = Calculator.Calculate("a", "α", layout);
        Assert.Equal(1.0, result);
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
        double result = Calculator.Calculate(source, target, layout);
        // Transposition is a timing error, not a key proximity error.
        // Cost is always 1 regardless of key positions.
        Assert.Equal(1.0, result);
    }
}
