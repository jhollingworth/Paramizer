using System;
using System.ComponentModel;

namespace Paramizer
{
    public class DefaultParamConverter : IUrlParamConverter
    {
        public object Deserialize(Type destinationType, string value)
        {
            var converter = TypeDescriptor.GetConverter(destinationType);

            if(converter == null)
            {
                return value;
            }

            return converter.ConvertFromString(value);
        }

        public string Serialize(object value)
        {
            return new TypeConverter().ConvertToString(value);
        }
    }
}