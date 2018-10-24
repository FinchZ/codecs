using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public static class TarsConvertExtensions
    {
        public static IServiceCollection AddTarsCodecs(this IServiceCollection services)
        {
            services.TryAddSingleton<ITarsHeadHandler, TarsHeadHandler>();
            services.TryAddSingleton<IDecoder<IByteBuffer>, TarsDecoder>();
            services.TryAddSingleton<IEncoder<IByteBuffer>, TarsEncoder>();
            services.TryAddSingleton<ITarsConvertRoot, TarsConvertRoot>();
            services.TryAddEnumerable<ITarsConvert<Request>, RequestTarsConvert>()
                .TryAddEnumerable<ITarsConvert<Response>, ResponseTarsConvert>()
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
                .TryAddEnumerable<ITarsConvert<double?>, NullableTarsConvert<double>>()
                .TryAddEnumerable<ITarsConvert<IByteBuffer>, ByteBufferTarsConvert>()
                .TryAddEnumerable<ITarsConvert<byte[]>, ByteArrayTarsConvert>()
                .TryAddEnumerable<ITarsConvert<Task>, TaskTarsConvert>();
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IListInterfaceTarsConvert<>), typeof(ListInterfaceTarsConvert<>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IListClassTarsConvert<>), typeof(ListClassTarsConvert<>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(ITaskTarsConvert<>), typeof(TaskResultTarsConvert<>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IValueTaskTarsConvert<>), typeof(ValueTaskTarsConvert<>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IStructTarsConvert<>), typeof(StructTarsConvert<>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IDictionaryInterfaceTarsConvert<,>), typeof(DictionaryInterfaceTarsConvert<,>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IDictionaryClassTarsConvert<,>), typeof(DictionaryClassTarsConvert<,>), ServiceLifetime.Singleton));
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IEnumTarsConvert<>), typeof(EnumTarsConvert<>), ServiceLifetime.Singleton));
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