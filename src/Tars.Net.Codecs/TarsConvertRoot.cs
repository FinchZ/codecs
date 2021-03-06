﻿using AspectCore.Extensions.Reflection;
using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Net.Codecs.Attributes;

namespace Tars.Net.Codecs
{
    public class TarsConvertRoot : ITarsConvertRoot
    {
        private readonly ConcurrentDictionary<(Codec, short, Type), (MethodReflector serialize, MethodReflector deserialize, object instance)> dict
            = new ConcurrentDictionary<(Codec, short, Type), (MethodReflector serialize, MethodReflector deserialize, object instance)>();

        private readonly IServiceProvider provider;

        public int Order => 0;

        public Codec Codec => Codec.Tars;

        public TarsConvertRoot(IServiceProvider provider)
        {
            this.provider = provider;
        }

        private (MethodReflector serialize, MethodReflector deserialize, object instance) GetConvert(Codec codec, Type type, TarsConvertOptions options)
        {
            return dict.GetOrAdd((codec, options.Version, type), (op) =>
            {
                Type convertType = GetConvertType(op.Item3);

                var convert = provider.GetServices(convertType).FirstOrDefault(i => ((ICanTarsConvert)i).Accept(op.Item1, op.Item2, op.Item3));
                if (convert == null)
                {
                    throw new NotSupportedException($"Codecs not supported type:{type}, options:{options}.");
                }
                convertType = convert.GetType();
                return (convertType.GetMethod("Serialize").GetReflector(), convertType.GetMethod("Deserialize").GetReflector(), convert);
            });
        }

        private static Type GetConvertType(Type convertType)
        {
            var type = convertType.IsByRef ? convertType.GetElementType() : convertType;

            var tInfo = type.GetTypeInfo();
            if (tInfo.IsTaskWithResult())
            {
                return typeof(ITaskTarsConvert<>).MakeGenericType(type.GetGenericArguments());
            }
            else if (tInfo.IsValueTask())
            {
                return typeof(IValueTaskTarsConvert<>).MakeGenericType(type.GetGenericArguments());
            }
            else if (tInfo.IsEnum)
            {
                return typeof(IEnumTarsConvert<>).MakeGenericType(type);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return typeof(IListInterfaceTarsConvert<>).MakeGenericType(type.GetGenericArguments());
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return typeof(IListClassTarsConvert<>).MakeGenericType(type.GetGenericArguments());
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                return typeof(IDictionaryInterfaceTarsConvert<,>).MakeGenericType(type.GetGenericArguments());
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return typeof(IDictionaryClassTarsConvert<,>).MakeGenericType(type.GetGenericArguments());
            }
            else if (type.IsClass && !type.IsAbstract && type.GetReflector().IsDefined<TarsStructAttribute>())
            {
                return typeof(IStructTarsConvert<>).MakeGenericType(type);
            }
            else
            {
                return typeof(ITarsConvert<>).MakeGenericType(type);
            }
        }

        public bool Accept((Codec, Type, short) options)
        {
            return true;
        }

        public void Serialize(object obj, Type type, IByteBuffer buffer, TarsConvertOptions options)
        {
            var op = options ?? new TarsConvertOptions();
            var (serialize, deserialize, instance) = GetConvert(op.Codec, type, op);
            serialize.Invoke(instance, obj, buffer, op);
        }

        public void Serialize<T>(T obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            Serialize(obj, typeof(T), buffer, options);
        }

        public object Deserialize(IByteBuffer buffer, Type type, TarsConvertOptions options)
        {
            var op = options ?? new TarsConvertOptions();
            var (serialize, deserialize, instance) = GetConvert(op.Codec, type, op);
            return deserialize.Invoke(instance, buffer, op);
        }

        public T Deserialize<T>(IByteBuffer buffer, TarsConvertOptions options)
        {
            return (T)Deserialize(buffer, typeof(T), options);
        }
    }
}