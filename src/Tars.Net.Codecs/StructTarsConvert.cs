using AspectCore.Extensions.Reflection;
using DotNetty.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Net.Codecs.Attributes;

namespace Tars.Net.Codecs
{
    public interface IStructTarsConvert<T> : ITarsConvert<T> where T : class, new()
    { }

    public class StructTarsConvert<T> : TarsConvertBase<T>, IStructTarsConvert<T> where T : class, new()
    {
        private readonly ITarsConvertRoot convert;
        private readonly ITarsHeadHandler headHandler;
        private readonly SortedDictionary<int, PropertyReflector> properties = new SortedDictionary<int, PropertyReflector>();

        public StructTarsConvert(ITarsConvertRoot convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
            var ps = typeof(T).GetProperties()
                .Where(i => i.CanRead
                    && i.CanWrite
                    && i.GetReflector().IsDefined<TarsStructPropertyAttribute>())
                .Select(i => (property: i.GetReflector(), info: i.GetReflector().GetCustomAttribute<TarsStructPropertyAttribute>()))
                .OrderBy(i => i.info.Order);
            foreach (var (property, info) in ps)
            {
                properties.Add(info.Order, property);
            }
        }

        public override T Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            if (options.TarsType == TarsStructType.StructBegin)
            {
                var op = options.Create();
                headHandler.ReadHead(buffer, op);
                var result = new T();
                while (op.TarsType != TarsStructType.StructEnd)
                {
                    var p = properties[op.Tag];
                    p.SetValue(result, convert.Deserialize(buffer, p.GetMemberInfo().PropertyType, op));
                    headHandler.ReadHead(buffer, op);
                }
                return result;
            }
            else
            {
                throw new TarsDecodeException($"StructTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(T obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj != null)
            {
                headHandler.Reserve(buffer, 2);
                headHandler.WriteHead(buffer, TarsStructType.StructBegin, options.Tag);
                foreach (var kv in properties)
                {
                    options.Tag = kv.Key;
                    convert.Serialize(kv.Value.GetValue(obj), kv.Value.GetMemberInfo().PropertyType, buffer, options);
                }
                headHandler.Reserve(buffer, 2);
                headHandler.WriteHead(buffer, TarsStructType.StructEnd, options.Tag);
            }
        }
    }
}