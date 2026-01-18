using HumanDistance.Keyboards;

namespace HumanDistance.Internal;

internal static class KeyboardDistanceCalculator
{
    public static double GetSubstitutionCost(char a, char b, KeyboardLayoutBase layout)
    {
        if (char.ToLowerInvariant(a) == char.ToLowerInvariant(b))
        {
            return 0.0;
        }

        bool foundA = layout.TryGetPosition(a, out float ax, out float ay);
        bool foundB = layout.TryGetPosition(b, out float bx, out float by);

        if (!foundA || !foundB)
        {
            return 1.0;
        }

        float dx = ax - bx;
        float dy = ay - by;
        float distance = MathF.Sqrt(dx * dx + dy * dy);

        return distance / layout.MaxDistance;
    }

    public static KeyboardLayoutBase CreateLayout(KeyboardLayout layout) => layout switch
    {
        KeyboardLayout.Qwerty => new QwertyLayout(),
        KeyboardLayout.Azerty => new AzertyLayout(),
        KeyboardLayout.Qwertz => new QwertzLayout(),
        _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, "Unknown keyboard layout")
    };
}
