using System.Runtime.Serialization;

namespace Rees.UnitTestUtilities;

[Serializable]
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

    protected EnsureFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}