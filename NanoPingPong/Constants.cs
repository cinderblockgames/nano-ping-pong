namespace NanoPingPong
{
    public static class Constants
    {

        public static class EnvironmentVariables
        {
            public const string Context = "Context";
            public const string SeedFile = "SeedFile";
            public const string TickSeconds = "TickSeconds";
            public const string Node = "Node";
            public const string WorkServer = "WorkServer";
        }

        public static class Locations
        {
            public const string Log = "/run/logs/output.log";
        }

        public static class Protocols
        {
            public static class Nano
            {
                public const string Prefix = "nano";
                public const string SendDifficulty = "fffffff800000000";
                public const string ReceiveDifficulty = "fffffe0000000000";
            }

            public static class Banano
            {
                public const string Prefix = "ban";
                public const string SendDifficulty = "fffffe0000000000";
                public const string ReceiveDifficulty = "fffffe0000000000";
            }
        }

    }
}
