using System;
using System.Collections.Generic;
using DockerHax.IO;
using Newtonsoft.Json.Linq;
using static NanoPingPong.Constants;
using Names = NanoPingPong.Constants.EnvironmentVariables;

namespace NanoPingPong
{
    public class Context
    {

        public string LogFile { get; }
        public string Seed { get; }
        public int TickSeconds { get; }
        public string Node { get; }
        public string WorkServer { get; }
        public string Prefix { get; }
        public string SendDifficulty { get; }
        public string ReceiveDifficulty { get; }

        public Context()
        {
            LogFile = Locations.Log;

            var env = GetEnvironmentVariables();

            Seed = JObject.Parse(File.ReadAllText(env[Names.SeedFile])).ToObject<NanoSeed>().Seed;
            TickSeconds = int.Parse(env[Names.TickSeconds]);
            Node = env[Names.Node];
            WorkServer = env[Names.WorkServer];

            var banano = string.Equals(env[Names.Context], nameof(Protocols.Banano));
            Prefix = banano ? Protocols.Banano.Prefix : Protocols.Nano.Prefix;
            SendDifficulty = banano ? Protocols.Banano.SendDifficulty : Protocols.Nano.SendDifficulty;
            ReceiveDifficulty = banano ? Protocols.Banano.ReceiveDifficulty : Protocols.Nano.ReceiveDifficulty;
        }

        private IDictionary<string, string> GetEnvironmentVariables()
        {
            var env = Environment.GetEnvironmentVariables();
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in env.Keys)
            {
                dic.Add(key, (string)env[key]);
            }
            return dic;
        }

    }
}
