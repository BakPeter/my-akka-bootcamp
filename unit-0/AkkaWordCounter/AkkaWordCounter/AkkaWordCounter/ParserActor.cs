using Akka.Actor;
using Akka.Event;
using static AkkaWordCounter.DocumentCommands;

namespace AkkaWordCounter;

public class ParserActor : UntypedActor
{
    private readonly ILoggingAdapter _log = Context.GetLogger();
    private readonly IActorRef _counterActor;
    private readonly int _tokenBatchSize = 10;

    public ParserActor(IActorRef counterActor)
    {
        _counterActor = counterActor;
    }

    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case ProcessDocument process:
            {
                foreach (var tokensBatch in process.RawText.Split(" ").Chunk(_tokenBatchSize))
                {
                    _counterActor.Tell(new CounterCommands.CountTokens(tokensBatch));
                }

                _counterActor.Tell(new CounterCommands.ExpectNoMoreTokens());
                break;
            }
            default:
            {
                Unhandled(message);
                break;
            }
        }
    }
}