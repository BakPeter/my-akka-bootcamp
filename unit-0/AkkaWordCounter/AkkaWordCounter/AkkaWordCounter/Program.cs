using Akka.Actor;
using Akka.Event;
using AkkaWordCounter;

var myActorSystem = ActorSystem.Create("myActorSystem");
myActorSystem.Log.Info("Hello from myActorSystem");

// set Actor configurations
var helloActorProps = Props.Create(() => new HelloActor());

// create an instance of HelloActor and get IActorRef to it
var helloActor = myActorSystem.ActorOf(helloActorProps, "helloActor");

// send message to the HalloActor
helloActor.Tell("Hello, my friend [tell]");

// Use Ask to do request-response with helloActor
var response = await helloActor.Ask<string>("Hello, my friend [ask]", TimeSpan.FromSeconds(3));
Console.WriteLine("Response from HelloActor: {0}", response);

await myActorSystem.Terminate();