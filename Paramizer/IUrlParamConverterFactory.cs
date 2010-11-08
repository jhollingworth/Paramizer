using System;

namespace Paramizer
{
    public interface IUrlParamConverterFactory
    {
        IUrlParamConverter Create(Type converterType);
    }
}