﻿namespace NanoPingPong.Shared.Config
{
    public interface IContext
    {

        string LogFile { get; }
        string Seed { get; }
        int TickMilliseconds { get; }
        string Node { get; }
        string WorkServer { get; }
        string Prefix { get; }
        string SendDifficulty { get; }
        string ReceiveDifficulty { get; }

    }
}