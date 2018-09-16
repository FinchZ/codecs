using System;

namespace Tars.Net.Codecs
{
    public interface ITarsConvertCreator
    {
        int Order { get; }

        bool Accept((Type, short) options);

        ITarsStreamConvert Create((Type, short) options);
    }
}