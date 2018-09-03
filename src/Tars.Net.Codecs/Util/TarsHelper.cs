using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tars.Net.Codecs.Attributes;
using Tars.Net.Codecs.Reflection;
using Tars.Net.Codecs.Support;

namespace Tars.Net.Codecs.Util
{
    class TarsHelper
    {
        private static readonly Type[] EmptyTypeArray = new Type[] { };
        private static ConcurrentDictionary<Type, TarsStructInfo> tarsStructCache = new ConcurrentDictionary<Type, TarsStructInfo>();
        //获取结构体详情
        internal static TarsStructInfo getStructInfo(Type type)
        {
            TarsStructInfo structInfo = tarsStructCache.GetOrAdd(type, t =>
            {
                TarsStructInfo tarsStructInfo = new TarsStructInfo();
                PropertyInfo[] fields = type.GetProperties(System.Reflection.BindingFlags.Public);
                if (fields.Length > 0)
                {
                    List<TarsStructPropertyInfo> propertyList = new List<TarsStructPropertyInfo>();
                    foreach (var item in fields)
                    {
                        TarsStructPropertyAttribute attri = item.GetCustomAttribute<TarsStructPropertyAttribute>();
                        if (attri != null)
                        {
                            TarsStructPropertyInfo propertyInfo = new TarsStructPropertyInfo();
                            Type fieldType = item.PropertyType;
                            propertyInfo.Name = item.Name;
                            propertyInfo.Order = attri.Order;
                            propertyInfo.IsRequire = attri.IsRequire;
                            propertyInfo.Comment = attri.Comment;
                            propertyInfo.PropertyAccessor = new PropertyAccessor(item);
                            propertyInfo.Type = fieldType;
                            propertyList.Add(propertyInfo);
                        }
                    }
                    TarsStructAttribute structAttribute = type.GetCustomAttribute<TarsStructAttribute>();
                    propertyList.Sort((x, y) => x.Order.CompareTo(y.Order));
                    tarsStructInfo.PropertyList = propertyList;
                    tarsStructInfo.ConstructorInvoker = new ConstructorInvoker(type.GetConstructor(EmptyTypeArray));
                    if (structAttribute != null)
                    {
                        string comment = structAttribute.Comment;
                        if (string.IsNullOrWhiteSpace(comment))
                        {
                            tarsStructInfo.Comment = comment;
                        }
                    }
                }
                return tarsStructInfo;
            });
            return structInfo;
        } 
    }
}
