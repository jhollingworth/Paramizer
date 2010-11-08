using System;

namespace Paramizer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UrlParamAttribute : Attribute
    {
        public bool Required { get; set; }
        public Type Converter { get; set; }
        public string Key { get; set; }

        public UrlParamAttribute()
        {
        }

        public UrlParamAttribute(string key, bool required)
        {
            Key = key;
            Required = required;
        }
    }
}
