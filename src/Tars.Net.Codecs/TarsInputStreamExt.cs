using System;
using System.Collections.Generic;
using Tars.Net.Codecs.Support;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs
{
    internal class TarsInputStreamExt
    {
        internal static object Read(Type type, int tag, bool isRequire, TarsInputStream jis)
        {
            TarsStructInfo info = TarsHelper.getStructInfo(type);
            if (info == null)
                throw new TarsDecodeException("the class type[" + type.FullName + "] no attribute Struct");
            if (jis.SkipToTag(tag))
            {
                HeadData hd = new HeadData();
                jis.ReadHead(hd);
                if (hd.Type != TarsStructBase.STRUCT_BEGIN)
                    throw new TarsDecodeException("type mismatch.");
                Object result = info.ConstructorInvoker.Invoke();
                List<TarsStructPropertyInfo> propertysList = info.PropertyList;
                foreach (var propertyInfo in propertysList)
                {
                    Object value = jis.Read(propertyInfo.Type, propertyInfo.Order, propertyInfo.IsRequire);
                    propertyInfo.PropertyAccessor.SetValue(result, value);
                }
                jis.SkipToStructEnd();
                return result;
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return null;

        }
    }
}