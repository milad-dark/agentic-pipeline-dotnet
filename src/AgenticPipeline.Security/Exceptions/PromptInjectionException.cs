namespace AgenticPipeline.Security.Exceptions;

public sealed class PromptInjectionException(string message) : Exception(message)
{
}
