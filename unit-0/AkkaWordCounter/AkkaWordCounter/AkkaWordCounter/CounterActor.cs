using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;
using static AkkaWordCounter.CounterCommands;
using static AkkaWordCounter.CounterQueries;

namespace AkkaWordCounter;

public class CounterActor : UntypedActor
{
    private readonly ILoggingAdapter _log = Context.GetLogger();
    private readonly Dictionary<string, int> _wordCounts = new();
    private bool _doneCounting = false;
    private readonly HashSet<IActorRef> _subscribers = new();

    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case CountTokens countTokens:
            {
                foreach (var token in countTokens.Tokens)
                {
                    if (!_wordCounts.TryAdd(token, 1))
                        _wordCounts[token] += 1;
                }

                break;
            }
            case ExpectNoMoreTokens:
            {
                _log.Info("Received ExpectNoMoreTokens - finished counting [{0}] unique tokens", _wordCounts.Count);
                _doneCounting = true;

                var totals = _wordCounts.ToImmutableDictionary();
                foreach (var subscriber in _subscribers)
                {
                    subscriber.Tell(totals);
                }

                _subscribers.Clear();
                break;
            }
            case FetchCounts fetchCount when _doneCounting:
            {
                _log.Info("Received FetchCount - sending [{0}] count to [{1}]for later", _wordCounts.Count,
                    fetchCount.Subscriber);
                _subscribers.Add(fetchCount.Subscriber);
                break;
            }
            case FetchCounts fetchCounts:
            {
                _log.Info("Received FetchCount - storing [{0}] for later", fetchCounts.Subscriber);
                _subscribers.Add(fetchCounts.Subscriber);
                break;
            }
            default:
                Unhandled(message);
                break;
        }
    }
}