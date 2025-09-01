using Akka.Actor;
using Akka.Event;
using AkkaWordCounter;

var myActorSystem = ActorSystem.Create("myActorSystem");
//myActorSystem.Log.Info("Hello from myActorSystem");

//// set Actor configurations
//var helloActorProps = Props.Create(() => new HelloActor());

//// create an instance of HelloActor and get IActorRef to it
//var helloActor = myActorSystem.ActorOf(helloActorProps, "helloActor");

//// send message to the HalloActor
//helloActor.Tell("Hello, my friend [tell]");

//// Use Ask to do request-response with helloActor
//var response = await helloActor.Ask<string>("Hello, my friend [ask]", TimeSpan.FromSeconds(3));
//Console.WriteLine("Response from HelloActor: {0}", response);


var counterActorProps = Props.Create(() => new CounterActor());
var counterActor = myActorSystem.ActorOf(counterActorProps, "counterActor");

var parserActorProps = Props.Create(() => new ParserActor(counterActor));
var parserActor = myActorSystem.ActorOf(parserActorProps, "parserActor");

Task<IDictionary<string, int>> completionPromise = counterActor.Ask<IDictionary<string, int>>(
    @ref => new CounterQueries.FetchCounts(@ref), null, CancellationToken.None);

parserActor.Tell(new DocumentCommands.ProcessDocument("""
                                                      This This This is a test of the Akka .NET Word Counter.
                                                      I would go
                                                      I Akka Akka n
                                                      I
                                                      """));

var wordCounts = await completionPromise;
foreach (var (word, count) in wordCounts)
{
    Console.WriteLine("{0}: {1} instances", word, count);
}
await myActorSystem.Terminate();