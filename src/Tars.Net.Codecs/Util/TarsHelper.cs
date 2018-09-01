using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tars.Net.Codecs.Attributes;
using Tars.Net.Codecs.Support;

namespace Tars.Net.Codecs.Util
{
    class TarsHelper
    {
        private static ConcurrentDictionary<Type, TarsStructInfo> tarsStructCache = new ConcurrentDictionary<Type, TarsStructInfo>();

        internal static TarsStructInfo getStructInfo(Type type)
        {
            TarsStructInfo info = tarsStructCache.GetOrAdd(type, t =>
            {
                TarsStructInfo tarsStructInfo = new TarsStructInfo();
                PropertyInfo[] fields = type.GetProperties(System.Reflection.BindingFlags.Public);
                if (fields.Length > 0)
                {
                    List<PropertyInfo> fieldList = new List<PropertyInfo>();
                    foreach (var item in fields)
                    {
                        TarsStructPropertyAttribute attri = item.GetCustomAttribute<TarsStructPropertyAttribute>();
                        if (attri != null) {
                            fieldList.Add(item);
                            TarsStructPropertyInfo propertyInfo = new TarsStructPropertyInfo();
                            

                        }
                    }
                    //sort
                    List<TarsStructPropertyInfo> propertyList = new List<TarsStructPropertyInfo>();
                    int order = 0;
                    Object obj = BasicClassTypeUtil.CreateObject(type);








                }
                return tarsStructInfo;
            });
            return info;
        }
    }
}
