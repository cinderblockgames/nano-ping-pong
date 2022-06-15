namespace NanoPingPong
{
    public static class Constants
    {

        public static class Locations
        {
            public const string Seed = "/run/secrets/nano-ping.seed";
            public const string WorkServer = "http://work-server-01.squeakers.space:7077";
#if DEBUG
            public const string Node = "http://host-x86-01:17076";
#else
            public const string Node = "http://nano_node:7076";
#endif
        }

        public static class Difficulty
        {
            public const string Send = "fffffff800000000";
            public const string Receive = "fffffe0000000000";
        }

    }
}
