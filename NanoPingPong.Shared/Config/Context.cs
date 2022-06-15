using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using static NanoPingPong.Shared.Config.Constants;
using Names = NanoPingPong.Shared.Config.Constants.EnvironmentVariables;

namespace NanoPingPong.Shared.Config
{
    internal class Context : IContext
    {

        public string LogFile => LogFileValue.GetValue();
        public string Seed => SeedValue.GetValue();
        public int TickMilliseconds => TickMillisecondsValue.GetValue();
        public string Node => NodeValue.GetValue();
        public string WorkServer => WorkServerValue.GetValue();
        public string Prefix => PrefixValue.GetValue();
        public string SendDifficulty => SendDifficultyValue.GetValue();
        public string ReceiveDifficulty => ReceiveDifficultyValue.GetValue();

        private IDictionary<string, string> Env { get; }
        private JustInTimeValue<bool> BananoValue { get; }
        private JustInTimeValue<string> LogFileValue { get; }
        private JustInTimeValue<string> SeedValue { get; }
        private JustInTimeValue<int> TickMillisecondsValue { get; }
        private JustInTimeValue<string> NodeValue { get; }
        private JustInTimeValue<string> WorkServerValue { get; }
        private JustInTimeValue<string> PrefixValue { get; }
        private JustInTimeValue<string> SendDifficultyValue { get; }
        private JustInTimeValue<string> ReceiveDifficultyValue { get; }

        public Context()
        {
            Env = GetEnvironmentVariables();

            BananoValue            = Build(() => string.Equals(Env[Names.Context], nameof(Protocols.Banano)));
            LogFileValue           = Build(() => Locations.Log);
            SeedValue              = Build(() => JObject.Parse(File.ReadAllText(Env[Names.SeedFile])).ToObject<NanoSeed>().Seed);
            TickMillisecondsValue  = Build(() => int.Parse(Env[Names.TickSeconds]) * 1000);
            NodeValue              = Build(() => Env[Names.Node]);
            WorkServerValue        = Build(() => Env[Names.WorkServer]);
            PrefixValue            = Build(() => BananoValue.GetValue() ? Protocols.Banano.Prefix : Protocols.Nano.Prefix);
            SendDifficultyValue    = Build(() => BananoValue.GetValue() ? Protocols.Banano.SendDifficulty : Protocols.Nano.SendDifficulty);
            ReceiveDifficultyValue = Build(() => BananoValue.GetValue() ? Protocols.Banano.ReceiveDifficulty : Protocols.Nano.ReceiveDifficulty);
        }

        private static IDictionary<string, string> GetEnvironmentVariables()
        {
            var env = Environment.GetEnvironmentVariables();
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in env.Keys)
            {
                dic.Add(key, (string)env[key]);
            }
            return dic;
        }

        private static JustInTimeValue<T> Build<T>(Func<T> getter)
        {
            return new JustInTimeValue<T>(getter);
        }

    }
}
