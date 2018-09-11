using System;
using System.Collections.Generic;
using Tars.Net.Codecs.Support;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs
{
    internal class TarsOutputStreamExt
    {
        internal static void Write(object e, int tag, TarsOutputStream jos)
        {
            TarsStructInfo info = TarsHelper.GetStructInfo(e.GetType());
            if (info == null)
                throw new TarsDecodeException("the class type[" + e.GetType().FullName + "] no attribute Struct");
            jos.WriteHead(TarsStructBase.STRUCT_BEGIN, tag);
            List<TarsStructPropertyInfo> propertysList = info.PropertyList;
            foreach (var propertyInfo in propertysList)
            {
                Object value = propertyInfo.PropertyAccessor.GetValue(e);
                if (value == null && propertyInfo.IsRequire)
                    throw new TarsEncodeException(propertyInfo.Name + " is require tag ,order=" + propertyInfo.Order);
                if (value != null)
                    jos.Write(value, propertyInfo.Order);
            }
            jos.WriteHead(TarsStructBase.STRUCT_END, 0);
        }
    }
}