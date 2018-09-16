using System;

namespace Tars.Net.Codecs
{
    public interface ITarsConvertCreatorRoot
    {
        ITarsStreamConvert Create((Type, short) options);
    }
}