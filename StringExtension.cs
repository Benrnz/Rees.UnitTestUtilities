namespace Rees.UnitTestUtilities;

/// <summary>
///     An extension class for string.
/// </summary>
public static class StringExtension
{
    /// <summary>
    ///     Returns true if the string is null, whitespace or empty.
    ///     Equivalent to IsNullOrWhiteSpace
    /// </summary>
    public static bool IsNothing(this string instance)
    {
        return string.IsNullOrWhiteSpace(instance);
    }

    /// <summary>
    ///     Returns true if the string is not null, not whitespace, and not empty.
    ///     The direct opporsite of <see cref="IsNothing" />
    /// </summary>
    public static bool IsSomething(this string instance)
    {
        return !string.IsNullOrWhiteSpace(instance);
    }

    /// <summary>
    ///     An extension to the <see cref="string.Split(char[])" /> method. This method splits at new line characters, trims
    ///     off trailing whitespace, and returns the specified number of lines.
    /// </summary>
    public static string[] SplitLines(this string instance, int numberOfLines = 0)
    {
        if (numberOfLines < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfLines), "Number of Lines must be a positive integer.");

        string[] split = instance.Split('\r', '\n');
        IEnumerable<string> query = split.Where(l => l.Length > 0);
        if (numberOfLines > 0) query = query.Take(numberOfLines);

        return query.ToArray();
    }
}
