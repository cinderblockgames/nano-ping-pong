using System;
using System.Timers;

namespace NanoPingPong
{
    public class Listener : IDisposable
    {

        private WrappedAccount Account { get; }
        public Listener(WrappedAccount account)
        {
            Account = account;
        }

        private Timer Timer { get; set; }
        public void Start(TimeSpan interval)
        {
            if (Timer == null)
            {
                Timer = new Timer(interval.TotalMilliseconds);
                Timer.Elapsed += (_, _) => Account.Tick();
                Timer.Start();
            }
        }

        public void Dispose()
        {
            using (Timer)
            {
                Timer.Stop();
            }
        }

    }
}
