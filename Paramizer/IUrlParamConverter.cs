using System;

namespace Paramizer
{
    public interface IUrlParamConverter
    {
        object Deserialize(Type destinationType, string value);
        string Serialize(object value);
    }
}