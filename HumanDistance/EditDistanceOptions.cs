namespace HumanDistance;

public sealed class EditDistanceOptions
{
    public bool UseKeyboardDistance { get; init; } = true;

    public KeyboardLayout Layout { get; init; } = KeyboardLayout.Qwerty;
}
