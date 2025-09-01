using Akka.Actor;

namespace AkkaWordCounter;

/// <summary>
/// Are for the parsing actor
/// </summary>
public static class DocumentCommands
{
    public sealed record ProcessDocument(string RawText);
}


/// <summary>
/// Are for the totaling actor
/// </summary>
public sealed class CounterCommands
{

    public sealed record CountTokens(IReadOnlyList<string> Tokens);

    public sealed class ExpectNoMoreTokens();
}


public static class CounterQueries
{
    /// <summary>
    /// Get the current count of tokens
    /// </summary>
    /// <param name="Subscriber">The actor who should be notified</param>
    public sealed record FetchCounts(IActorRef Subscriber);
}