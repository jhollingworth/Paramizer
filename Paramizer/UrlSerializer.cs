using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Paramizer
{
    public interface IUrlSerializer
    {
        Uri Serialize(string baseUrl, object entity);
        TEntity Deserialize<TEntity>(Uri url);
    }

    public class UrlSerializer : IUrlSerializer
    {
        private readonly IUrlParamConverterFactory _converterFactory;
        private IUrlParamConverter _defaultParamConverter;

        public UrlSerializer()
            :this(new DefaultUrlParamConverterFactory())
        {
        }

        public UrlSerializer(IUrlParamConverterFactory converterFactory)
        {
            _converterFactory = converterFactory;
            _defaultParamConverter = new DefaultParamConverter();
        }

        public Uri Serialize(string baseUrl, object entity)
        {
            if (false == baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            return new Uri(baseUrl + GetQueryString(entity));
        }

        private string GetQueryString(object entity)
        {
            var properties = entity.GetType().GetProperties();
            var @params = new Dictionary<string, string>();
            
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof (UrlParamAttribute), false);

                foreach (var attribute in attributes.Cast<UrlParamAttribute>())
                {
                    var key = string.IsNullOrEmpty(attribute.Key) ? property.Name : attribute.Key;
                    var value = property.GetValue(entity, new object[0]);
                    var converter = GetConverter(attribute);

                    @params[key] = converter.Serialize(value);
                }
            }

            var qs = @params.Aggregate("?", (current, param) => current + string.Format("{0}={1}&", param.Key, param.Value));

            return qs.Substring(0, qs.Length - 1);
        }

        private IUrlParamConverter GetConverter(UrlParamAttribute attribute)
        {
            var converter = _defaultParamConverter;
            if(attribute.Converter != null)
            {
                converter = _converterFactory.Create(attribute.Converter);
            }
            return converter;
        }

        public TEntity Deserialize<TEntity>(Uri url)
        {
            var @params = HttpUtility.ParseQueryString(url.Query.ToLower());
            var entity = (TEntity)Activator.CreateInstance(typeof (TEntity));

            foreach (var property in typeof(TEntity).GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof (UrlParamAttribute), false).Cast<UrlParamAttribute>();

                foreach (var attribute in attributes)
                {
                    var key = (string.IsNullOrEmpty(attribute.Key) ? property.Name : attribute.Key).ToLower();
                    var value = @params[key];
                    var converter = GetConverter(attribute);

                    if (string.IsNullOrEmpty(value) && attribute.Required)
                    {
                        throw new SerializationException(string.Format("The url {0} did not have the query string parameter {1}", url.Query, key));
                    }

                    property.SetValue(entity, converter.Deserialize(property.PropertyType, value), new object[0]);
                }
            }

            return entity;
        }
    }
}