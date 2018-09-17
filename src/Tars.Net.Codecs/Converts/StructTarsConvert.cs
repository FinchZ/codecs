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
        private readonly SortedDictionary<int, PropertyReflector> properties = new SortedDictionary<int, PropertyReflector>();

        public StructTarsConvert(ITarsConvertRoot convert)
        {
            this.convert = convert;
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
            if (options.TarsType == TarsStructBase.STRUCT_BEGIN)
            {
                ReadHead(buffer, options);
            }

            if (options.TarsType == TarsStructBase.STRUCT_END)
            {
                return null;
            }

            var result = new T();
            while (options.TarsType != TarsStructBase.STRUCT_END)
            {
                var p = properties[options.Tag];
                p.SetValue(result, convert.Deserialize(buffer, p.GetMemberInfo().PropertyType, options));
                ReadHead(buffer, options);
            }
            return result;
        }

        public override void Serialize(T obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            Reserve(buffer, 2);
            WriteHead(buffer, TarsStructBase.STRUCT_BEGIN, options.Tag);
            if (obj != null)
            {
                foreach (var kv in properties)
                {
                    options.Tag = kv.Key;
                    convert.Serialize(kv.Value.GetValue(obj), buffer, options);
                }
            }
            Reserve(buffer, 2);
            WriteHead(buffer, TarsStructBase.STRUCT_END, options.Tag);
        }
    }
}