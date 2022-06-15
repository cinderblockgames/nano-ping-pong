using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace NanoPingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up; please wait.");

            var provider = new ServiceCollection().AddDependencies().BuildServiceProvider();

            var listener = provider.GetRequiredService<Listener>();
            listener.Start(TimeSpan.FromSeconds(1));
            
            var stop = new ManualResetEventSlim();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => stop.Set();
            Console.WriteLine("Listener running.  Awaiting SIGTERM.");

            using (listener)
            {
                stop.Wait();
                Console.WriteLine("Shutting down; please wait.");
            }
            Console.WriteLine("Shutdown complete.");
        }
    }
}
