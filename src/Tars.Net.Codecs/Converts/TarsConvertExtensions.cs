using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public static class TarsConvertExtensions
    {
        public static IServiceCollection AddTarsCodecs(this IServiceCollection services)
        {
            services.TryAddSingleton<ITarsConvertRoot, TarsConvertRoot>();
            services.TryAddEnumerable<ITarsConvert<Request>, RequestTarsConvert>()
                .TryAddEnumerable<ITarsConvert<byte>, ByteTarsConvert>()
                .TryAddEnumerable<ITarsConvert<bool>, BoolTarsConvert>()
                .TryAddEnumerable<ITarsConvert<short>, ShortTarsConvert>()
                .TryAddEnumerable<ITarsConvert<int>, IntTarsConvert>()
                .TryAddEnumerable<ITarsConvert<long>, LongTarsConvert>()
                .TryAddEnumerable<ITarsConvert<float>, FloatTarsConvert>()
                .TryAddEnumerable<ITarsConvert<double>, DoubleTarsConvert>()
                .TryAddEnumerable<ITarsConvert<string>, StringTarsConvert>()
                .TryAddEnumerable<ITarsConvert<byte?>, NullableTarsConvert<byte>>()
                .TryAddEnumerable<ITarsConvert<bool?>, NullableTarsConvert<bool>>()
                .TryAddEnumerable<ITarsConvert<short?>, NullableTarsConvert<short>>()
                .TryAddEnumerable<ITarsConvert<int?>, NullableTarsConvert<int>>()
                .TryAddEnumerable<ITarsConvert<long?>, NullableTarsConvert<long>>()
                .TryAddEnumerable<ITarsConvert<float?>, NullableTarsConvert<float>>()
                .TryAddEnumerable<ITarsConvert<double?>, NullableTarsConvert<double>>();
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