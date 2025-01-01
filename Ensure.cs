namespace Rees.UnitTestUtilities;

public static class Ensure
{
    public static void AreEqualWithTolerance(decimal expected, decimal actual, decimal tolerance = 0.01M,
        string message = "")
    {
        if (Math.Abs(expected - actual) >= tolerance)
            throw new EnsureFailedException($"Expected: {expected}, Actual: {actual}. {message}");
    }

    public static void AreEqualWithTolerance(double expected, double actual, double tolerance = 0.001,
        string message = "")
    {
        if (Math.Abs(expected - actual) >= tolerance)
            throw new EnsureFailedException($"Expected: {expected}, Actual: {actual}. {message}");
    }
}