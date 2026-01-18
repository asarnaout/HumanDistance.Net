namespace HumanDistance;

public sealed class CalculatorOptions
{
    public bool UseKeyboardDistance { get; init; } = true;

    public KeyboardLayout Layout { get; init; } = KeyboardLayout.Qwerty;
}
