using System.Collections.Immutable;
using Google.Protobuf.WellKnownTypes;

namespace AkkaWordCounter;

public readonly record struct AbsoluteUri
{
    public AbsoluteUri(Uri value)
    {
        if (value.IsAbsoluteUri is false)
        {
            throw new ArgumentException("Value must be absolut", nameof(value));
        }

        Value = value;
    }

    public Uri Value { get; }

    public override string ToString() => Value.ToString();
}

public interface IWithDocumentId
{
    public AbsoluteUri DocumentId { get; }
}

public static class DocumentCommands
{
    public sealed record ScanDocument(AbsoluteUri DocumentId) : IWithDocumentId;
    public sealed record ScanDocuments(IReadOnlyList<AbsoluteUri> DocumentIds);
}

public static class DocumentEvents
{
    public sealed record DocumentScanFailed(AbsoluteUri DocumentId, string Reason) : IWithDocumentId;
    public sealed record WordsFound(AbsoluteUri DocumentId, IReadOnlyList<string> Tokens) : IWithDocumentId;
    public sealed record EndOfDocumentReached(AbsoluteUri DocumentId) : IWithDocumentId;
    public sealed record CountsTabulatedForDocument(AbsoluteUri DocumentId, ImmutableDictionary<string, int> WordsFrequencies) : IWithDocumentId;
    public sealed record CountsTabulatedForDocuments(IReadOnlyList<AbsoluteUri> DocumentIds, ImmutableDictionary<string, int> WordsFrequencies);
}

public static class DocumentQueries
{
    public sealed record FetchCounts(AbsoluteUri DocumentId) : IWithDocumentId;

    public sealed class SubscribeToAllCounts
    {
        public static readonly SubscribeToAllCounts Instance = new();

        private SubscribeToAllCounts() { }
    }
}