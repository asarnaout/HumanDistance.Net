namespace HumanDistance.Keyboards;

public interface IKeyboardLayout
{
    bool TryGetPosition(char c, out float x, out float y);
    float MaxDistance { get; }
}
