using Microsoft.Extensions.DependencyInjection;
using Nano.Net;

namespace NanoPingPong.Shared.Config
{
    public static class Dependencies
    {

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IContext, Context>();

            services.AddSingleton(provider => {
                var env = provider.GetRequiredService<IContext>();
                return new Account(env.Seed, 0, env.Prefix);
            });

            return services;
        }

    }
}
