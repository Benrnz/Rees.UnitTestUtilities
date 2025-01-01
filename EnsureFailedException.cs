using System.Runtime.Serialization;

namespace Rees.UnitTestUtilities;

public class EnsureFailedException : Exception
{
    public EnsureFailedException()
    {
    }

    public EnsureFailedException(string message) : base(message)
    {
    }

    public EnsureFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
