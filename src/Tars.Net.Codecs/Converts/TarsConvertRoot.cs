using AspectCore.Extensions.Reflection;
using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Tars.Net.Codecs
{
    public class TarsConvertRoot : ITarsConvertRoot
    {
        private readonly ConcurrentDictionary<(Codec, short), (MethodReflector serialize, MethodReflector deserialize, object instance)> dict
            = new ConcurrentDictionary<(Codec, short), (MethodReflector serialize, MethodReflector deserialize, object instance)>();

        private readonly IServiceProvider provider;

        public int Order => 0;

        public Codec Codec => Codec.Tars;

        public TarsConvertRoot(IServiceProvider provider)
        {
            this.provider = provider;
        }

        private (MethodReflector serialize, MethodReflector deserialize, object instance) GetConvert(Codec codec, Type type, TarsConvertOptions options)
        {
            return dict.GetOrAdd((codec, options.Version), (op) =>
            {
                var convert = provider.GetServices(type).FirstOrDefault(i => ((ICanTarsConvert)i).Accept(op.Item1, op.Item2));
                if (convert == null)
                {
                    throw new NotSupportedException($"Codecs not supported {options}.");
                }

                return (type.GetMethod("Serialize").GetReflector(), type.GetMethod("Deserialize").GetReflector(), convert);
            });
        }

        public bool Accept((Codec, Type, short) options)
        {
            return true;
        }

        public void Serialize<T>(T obj, IByteBuffer buffer, int order, TarsConvertOptions options)
        {
            var op = options ?? new TarsConvertOptions();
            var ( serialize,  deserialize,  instance) = GetConvert(op.Codec, typeof(T), op);
            serialize.Invoke(instance, obj, buffer, order, op);
        }

        public (int order, T value) Deserialize<T>(IByteBuffer buffer, TarsConvertOptions options)
        {
            var op = options ?? new TarsConvertOptions();
            var (serialize, deserialize, instance) = GetConvert(op.Codec, typeof(T), op);
            return ((int order, T value))serialize.Invoke(instance, buffer, op);
        }
    }
}