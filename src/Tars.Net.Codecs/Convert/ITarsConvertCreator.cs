using System;

namespace Tars.Net.Codecs
{
    public interface ITarsConvertCreator
    {
        short Version { get; }

        ITarsStreamConvert Create(Type type);
    }
}