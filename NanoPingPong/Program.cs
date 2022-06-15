using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace NanoPingPong
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting up; please wait.");

            var listener = new ServiceCollection().AddDependencies()
                                                  .BuildServiceProvider()
                                                  .GetRequiredService<Listener>();
            
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
