using Microsoft.Extensions.DependencyInjection;
using Nano.Net;

namespace NanoPingPong
{
    public static class Dependencies
    {

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<Context>();
            services.AddSingleton(provider =>
            {
                var env = provider.GetRequiredService<Context>();
                return new RpcClients
                {
                    Node = new RpcClient(env.Node),
                    WorkServer = new RpcClient(env.WorkServer)
                };
            });

            services.AddSingleton(provider => {
                var env = provider.GetRequiredService<Context>();
                return new Account(env.Seed, 0);
            });

            services.AddSingleton<WrappedAccount>();
            services.AddSingleton<Listener>();
            return services;
        }

    }
}
