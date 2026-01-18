namespace HumanDistance.Keyboards;

public sealed class AzertyLayout : KeyboardLayoutBase
{
    protected override Dictionary<char, (float X, float Y)> BuildPositions()
    {
        var positions = new Dictionary<char, (float X, float Y)>();

        // Row 0: Number row (y = 0, no x offset)
        // French AZERTY: ²&é"'(-è_çà)=
        const string row0 = "²1234567890°+";
        for (int i = 0; i < row0.Length; i++)
        {
            positions[row0[i]] = (i, 0);
        }

        // Row 1: AZERTY row (y = 1, x offset = 0.5)
        const string row1 = "azertyuiop^$";
        for (int i = 0; i < row1.Length; i++)
        {
            positions[row1[i]] = (i + 0.5f, 1);
        }

        // Row 2: Home row (y = 2, x offset = 0.75)
        const string row2 = "qsdfghjklm*";
        for (int i = 0; i < row2.Length; i++)
        {
            positions[row2[i]] = (i + 0.75f, 2);
        }

        // Row 3: Bottom row (y = 3, x offset = 1.25)
        const string row3 = "wxcvbn,;:!";
        for (int i = 0; i < row3.Length; i++)
        {
            positions[row3[i]] = (i + 1.25f, 3);
        }

        return positions;
    }
}
