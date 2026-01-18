namespace HumanDistance.Keyboards;

/// <summary>
/// A custom keyboard layout with user-defined key positions.
/// Use <see cref="CreateBuilder"/> to create a new custom layout.
/// </summary>
public sealed class CustomKeyboardLayout : KeyboardLayoutBase
{
    [ThreadStatic]
    private static Dictionary<char, (float X, float Y)>? _pendingPositions;

    private CustomKeyboardLayout() { }

    /// <summary>
    /// Creates a new builder for defining a custom keyboard layout.
    /// </summary>
    /// <returns>A new <see cref="Builder"/> instance.</returns>
    public static Builder CreateBuilder() => new();

    protected override Dictionary<char, (float X, float Y)> BuildPositions()
        => _pendingPositions ?? [];

    /// <summary>
    /// A fluent builder for creating custom keyboard layouts.
    /// </summary>
    public sealed class Builder
    {
        private readonly Dictionary<char, (float X, float Y)> _positions = [];

        internal Builder() { }

        /// <summary>
        /// Adds a row of characters to the keyboard layout.
        /// </summary>
        /// <param name="characters">The characters in this row, from left to right.</param>
        /// <param name="y">The vertical position of the row (0 = top row).</param>
        /// <param name="xOffset">The horizontal offset for this row (default 0).</param>
        /// <returns>This builder instance for method chaining.</returns>
        public Builder AddRow(string characters, float y, float xOffset = 0f)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                char c = char.ToLowerInvariant(characters[i]);
                _positions[c] = (i + xOffset, y);
            }
            return this;
        }

        /// <summary>
        /// Builds the keyboard layout from the configured rows.
        /// </summary>
        /// <returns>A <see cref="CustomKeyboardLayout"/> instance with the configured key positions.</returns>
        public CustomKeyboardLayout Build()
        {
            _pendingPositions = new Dictionary<char, (float X, float Y)>(_positions);
            try
            {
                return new CustomKeyboardLayout();
            }
            finally
            {
                _pendingPositions = null;
            }
        }
    }
}
