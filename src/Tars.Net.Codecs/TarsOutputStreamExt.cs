using System;
using Tars.Net.Codecs.Support;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs
{
    internal class TarsOutputStreamExt
    {
        internal static void Write(object e, int tag, TarsOutputStream tarsOutputStream)
        {
            TarsStructInfo info = TarsHelper.getStructInfo(e.GetType());
            if (info == null)
            {

                throw new TarsEncodeException("the JavaBean[" + e.GetType().Name + "] no annotation Struct");
            }
        }
    }
}