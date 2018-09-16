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

        public static IByteBuffer Serialize(this ITarsConvert convert, object obj, int order,
            bool isRequire = true, TarsConvertOptions options = null)
        {
            var buffer = Unpooled.Buffer(128);
            convert.Serialize(obj, buffer, order, isRequire, options);
            return buffer;
        }

        public static IServiceCollection AddTarsCodecs(this IServiceCollection services)
        {
            services.TryAddSingleton<ITarsConvertRoot, TarsConvertRoot>();
            services.TryAddEnumerable<ITarsConvert, RequestTarsConvert>()
                .TryAddEnumerable<ITarsConvert, ByteTarsConvert>()
                .TryAddEnumerable<ITarsConvert, BoolTarsConvert>();
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