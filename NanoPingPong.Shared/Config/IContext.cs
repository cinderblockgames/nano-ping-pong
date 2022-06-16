using Nano.Net;

namespace NanoPingPong.Shared.Config
{
    public interface IContext
    {

        bool Banano { get; }
        string LogFile { get; }
        string Seed { get; }
        int TickMilliseconds { get; }
        string Node { get; }
        string WorkServer { get; }
        string Prefix { get; }
        string SendDifficulty { get; }
        string ReceiveDifficulty { get; }
        string DonationAddress { get; }
        string Protocol { get; }
        string Link { get; }
        string DonationLink { get; }
        Account Account { get; }

    }
}
