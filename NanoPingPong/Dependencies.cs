using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Nano.Net;
using Newtonsoft.Json.Linq;
using static NanoPingPong.Constants;

namespace NanoPingPong
{
    public static class Dependencies
    {

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new RpcClients
                {
                    Node = new RpcClient(Locations.Node),
                    WorkServer = new RpcClient(Locations.WorkServer)
                }
            );

            var seed = JObject.Parse(File.ReadAllText(Locations.Seed)).ToObject<NanoSeed>().Seed;
            services.AddSingleton(provider => new Account(seed, 0));

            services.AddSingleton<WrappedAccount>();
            services.AddSingleton<Listener>();
            return services;
        }

    }
}
