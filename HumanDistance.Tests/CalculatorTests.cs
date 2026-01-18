namespace HumanDistance.Tests;

public class CalculatorTests
{
    [Fact]
    public void Calculate_IdenticalStrings_ReturnsZero()
    {
        int result = Calculator.Calculate("hello", "hello");
        Assert.Equal(0, result);
    }

    [Fact]
    public void Calculate_EmptyStrings_ReturnsZero()
    {
        int result = Calculator.Calculate("", "");
        Assert.Equal(0, result);
    }

    [Fact]
    public void Calculate_EmptySource_ReturnsTargetLength()
    {
        int result = Calculator.Calculate("", "hello");
        Assert.Equal(5, result);
    }

    [Fact]
    public void Calculate_EmptyTarget_ReturnsSourceLength()
    {
        int result = Calculator.Calculate("hello", "");
        Assert.Equal(5, result);
    }

    [Fact]
    public void Calculate_SingleInsertion_ReturnsOne()
    {
        int result = Calculator.Calculate("hell", "hello");
        Assert.Equal(1, result);
    }

    [Fact]
    public void Calculate_SingleDeletion_ReturnsOne()
    {
        int result = Calculator.Calculate("hello", "hell");
        Assert.Equal(1, result);
    }

    [Fact]
    public void Calculate_SingleSubstitution_ReturnsOne()
    {
        int result = Calculator.Calculate("hello", "hallo");
        Assert.Equal(1, result);
    }

    [Fact]
    public void Calculate_Transposition_ReturnsOne()
    {
        int result = Calculator.Calculate("hello", "hlelo");
        Assert.Equal(1, result);
    }

    [Fact]
    public void Calculate_MultipleEdits_ReturnsCorrectDistance()
    {
        int result = Calculator.Calculate("kitten", "sitting");
        Assert.Equal(3, result);
    }

    [Fact]
    public void Calculate_WithOptions_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Calculator.Calculate("hello", "world", null!));
    }

    [Fact]
    public void Calculate_WithOptions_UseKeyboardDistanceFalse_ReturnsIntegerDistance()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = false };
        double result = Calculator.Calculate("hello", "hallo", options);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Calculate_WithOptions_IdenticalStrings_ReturnsZero()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwerty };
        double result = Calculator.Calculate("hello", "hello", options);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithOptions_AdjacentKeys_ReturnsLowCost()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwerty };
        double result = Calculator.Calculate("a", "s", options);
        Assert.True(result < 0.5, $"Adjacent keys 'a' and 's' should have low cost, got {result}");
    }

    [Fact]
    public void Calculate_WithOptions_DistantKeys_ReturnsHigherCost()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwerty };
        double adjacentResult = Calculator.Calculate("a", "s", options);
        double distantResult = Calculator.Calculate("a", "p", options);
        Assert.True(distantResult > adjacentResult,
            $"Distant keys should have higher cost than adjacent keys. Adjacent: {adjacentResult}, Distant: {distantResult}");
    }

    [Fact]
    public void Calculate_WithOptions_QwertyLayout_WorksCorrectly()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwerty };
        double result = Calculator.Calculate("qwerty", "qwerty", options);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithOptions_AzertyLayout_WorksCorrectly()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Azerty };
        double result = Calculator.Calculate("azerty", "azerty", options);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithOptions_QwertzLayout_WorksCorrectly()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwertz };
        double result = Calculator.Calculate("qwertz", "qwertz", options);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithOptions_CaseInsensitive()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwerty };
        double result = Calculator.Calculate("Hello", "hello", options);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_WithOptions_UnknownCharacters_DefaultToFullCost()
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = KeyboardLayout.Qwerty };
        double result = Calculator.Calculate("a", "α", options);
        Assert.Equal(1.0, result);
    }

    [Theory]
    [InlineData("", "", 0)]
    [InlineData("a", "", 1)]
    [InlineData("", "a", 1)]
    [InlineData("abc", "abc", 0)]
    [InlineData("abc", "ab", 1)]
    [InlineData("ab", "abc", 1)]
    [InlineData("abc", "adc", 1)]
    [InlineData("abc", "acb", 1)]
    public void Calculate_VariousInputs_ReturnsExpectedDistance(string source, string target, int expected)
    {
        int result = Calculator.Calculate(source, target);
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
    public void Calculate_WithKeyboardDistance_VariousInputs_ReturnsExpectedDistance(
        KeyboardLayout layout, string source, string target, double expected)
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = layout };
        double result = Calculator.Calculate(source, target, options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwerty, "q", "w", "q", "m")]
    [InlineData(KeyboardLayout.Azerty, "a", "z", "a", "p")]
    [InlineData(KeyboardLayout.Azerty, "q", "s", "q", "m")]
    [InlineData(KeyboardLayout.Qwertz, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwertz, "q", "w", "q", "m")]
    public void Calculate_WithKeyboardDistance_AdjacentKeysLowerCostThanDistantKeys(
        KeyboardLayout layout, string source1, string adjacentTarget, string source2, string distantTarget)
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = layout };
        double adjacentResult = Calculator.Calculate(source1, adjacentTarget, options);
        double distantResult = Calculator.Calculate(source2, distantTarget, options);
        Assert.True(adjacentResult < distantResult,
            $"Layout {layout}: Adjacent keys ({source1}->{adjacentTarget}={adjacentResult:F3}) should have lower cost than distant keys ({source2}->{distantTarget}={distantResult:F3})");
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void Calculate_WithKeyboardDistance_UnknownCharacters_DefaultToFullCost(KeyboardLayout layout)
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = layout };
        double result = Calculator.Calculate("a", "α", options);
        Assert.Equal(1.0, result);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "ab", "ba")]
    [InlineData(KeyboardLayout.Qwerty, "qp", "pq")]
    [InlineData(KeyboardLayout.Azerty, "az", "za")]
    [InlineData(KeyboardLayout.Azerty, "qm", "mq")]
    [InlineData(KeyboardLayout.Qwertz, "ab", "ba")]
    [InlineData(KeyboardLayout.Qwertz, "qp", "pq")]
    public void Calculate_WithKeyboardDistance_Transposition_ReturnsOne(
        KeyboardLayout layout, string source, string target)
    {
        var options = new CalculatorOptions { UseKeyboardDistance = true, Layout = layout };
        double result = Calculator.Calculate(source, target, options);
        // Transposition is a timing error, not a key proximity error.
        // Cost is always 1 regardless of key positions.
        Assert.Equal(1.0, result);
    }
}
