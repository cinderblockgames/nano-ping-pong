using System;
using System.Timers;
using NanoPingPong.Shared.Config;

namespace NanoPingPong
{
    public class Listener : IDisposable
    {

        private Timer Timer { get; }

        public Listener(WrappedAccount account, IContext context)
        {
            Timer = new Timer(context.TickMilliseconds);
            Timer.Elapsed += (_, _) => account.Tick();
            Timer.Start();
        }

        public void Dispose()
        {
            using (Timer)
            {
                Timer.Stop();
            }
            GC.SuppressFinalize(this);
        }

    }
}
