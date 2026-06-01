namespace AgenticPipeline.Security.Exceptions;

public sealed class SecretLeakageException(string message) : Exception(message)
{
}
