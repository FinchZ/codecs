using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tars.Net.Codecs
{
    public static class TarsConvertExtensions
    {
        public static T Deserialize<T>(this ITarsConvert convert, IByteBuffer buffer, int order,
            bool isRequire = true, TarsConvertOptions options = null)
        {
            return (T)convert.Deserialize(buffer, typeof(T), order, isRequire, options);
        }

        public static IServiceCollection AddTarsCodecs(this IServiceCollection services)
        {
            services.TryAddSingleton<ITarsConvertRoot, TarsConvertRoot>();
            services.TryAddEnumerable<ITarsConvert, TarsRequestConvert>();
            return services;
        }

        public static IServiceCollection TryAddEnumerable<TService, TImplementation>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
            return services;
        }
    }
}