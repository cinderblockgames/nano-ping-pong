using Microsoft.Extensions.DependencyInjection;
using N2.Pow;
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
                return new RpcClient(env.Node);
            });

            services.AddSingleton(provider =>
            {
                var env = provider.GetRequiredService<IContext>();
                return new WorkServer(
                    new WorkServerOptions
                    {
                        ApiKey = env.N2ApiKey
                    }
                );
            });

            services.AddSingleton<WrappedAccount>();
            services.AddSingleton<Listener>();
            return services;
        }

    }
}
