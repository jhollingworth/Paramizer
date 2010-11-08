using System;

namespace Paramizer
{
    public class DefaultUrlParamConverterFactory : IUrlParamConverterFactory
    {
        public IUrlParamConverter Create(Type converterType)
        {
            return (IUrlParamConverter)Activator.CreateInstance(converterType, new object[0]);
        }
    }
}