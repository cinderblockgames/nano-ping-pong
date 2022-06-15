using Microsoft.Extensions.DependencyInjection;
using Nano.Net;
using NanoPingPong.Shared.Config;
using SharedDependencies = NanoPingPong.Shared.Config.Dependencies;

namespace NanoPingPong
{
    public static class Dependencies
    {

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            SharedDependencies.AddDependencies(services);

            services.AddSingleton(provider =>
            {
                var env = provider.GetRequiredService<IContext>();
                return new RpcClients
                {
                    Node = new RpcClient(env.Node),
                    WorkServer = new RpcClient(env.WorkServer)
                };
            });

            services.AddSingleton(provider => {
                var env = provider.GetRequiredService<IContext>();
                return new Account(env.Seed, 0);
            });

            services.AddSingleton<WrappedAccount>();
            services.AddSingleton<Listener>();
            return services;
        }

    }
}
