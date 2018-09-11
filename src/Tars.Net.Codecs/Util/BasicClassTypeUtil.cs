using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs.Util
{
    internal class BasicClassTypeUtil
    {
        /**
         * 将嵌套的类型转成字符串
         * @param listTpye
         * @return
         */

        public static string TransTypeList(List<string> listTpye)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listTpye.Count; i++)
            {
                listTpye[i] = CS2UniType(listTpye[i]);
            }
            listTpye.Reverse();
            for (int i = 0; i < listTpye.Count; i++)
            {
                string type = listTpye[i];
                if (type.Equals("list"))
                {
                    listTpye[i - 1] = "<" + listTpye[i - 1];
                    listTpye[0] = listTpye[0] + ">";
                }
                else if (type.Equals("map"))
                {
                    listTpye[i - 1] = "<" + listTpye[i - 1] + ",";
                    listTpye[0] = listTpye[0] + ">";
                }
                else if (type.Equals("Array"))
                {
                    listTpye[i - 1] = "<" + listTpye[i - 1];
                    listTpye[0] = listTpye[0] + ">";
                }
            }
            listTpye.Reverse();
            foreach (string s in listTpye)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }
         


        public static string CS2UniType(string srcType)
        {
            if (srcType.Equals("System.Int16"))
            {
                return "short";
            }
            else if (srcType.Equals("System.Int32"))
            {
                return "int32";
            }
            else if (srcType.Equals("System.Boolean"))
            {
                return "bool";
            }
            else if (srcType.Equals("System.Byte"))
            {
                return "char";
            }
            else if (srcType.Equals("System.Double"))
            {
                return "double";
            }
            else if (srcType.Equals("System.Single"))
            {
                return "float";
            }
            else if (srcType.Equals("System.Int64"))
            {
                return "int64";
            }
            else if (srcType.Equals("System.String"))
            {
                return "string";
            } 
            else
            {
                return srcType;
            }
        }

      

    }
}