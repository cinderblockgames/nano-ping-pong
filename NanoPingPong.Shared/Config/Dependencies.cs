using Microsoft.Extensions.DependencyInjection;

namespace NanoPingPong.Shared.Config
{
    public static class Dependencies
    {

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IContext, Context>();
            return services;
        }

    }
}
