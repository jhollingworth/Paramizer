using System;
using Machine.Specifications;
using Paramizer.Specs.Assertations;

namespace Paramizer.Specs
{
    public class Sample
    {
        [UrlParam]
        public string Param { get; set; }

        [UrlParam("k", true)]
        public int RequiredWithKey { get; set; }

        [UrlParam("t", true, Converter = typeof(ComplexEnumConverter))]
        public ComplexEnum Type { get; set; }
    }

    public class ComplexEnumConverter : IUrlParamConverter
    {
        public object Deserialize(Type destinationType, string value)
        {
            switch (value.ToLower())
            {
                case "f": return ComplexEnum.Foo;
                case "b": return ComplexEnum.Bar;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public string Serialize(object value)
        {
            return value.ToString().ToLower()[0].ToString();
        }
    }

    public enum ComplexEnum
    {
        Foo, Bar
    }

    public class When_I_serialize_an_object_to_a_url 
    {
        static Sample Subject;
        static Uri Result;
        static UrlSerializer Serializer;
        
        Establish context = () =>
        {
            Subject = new Sample { Param = "Foo", RequiredWithKey = 10, Type = ComplexEnum.Bar };
            Serializer = new UrlSerializer();
        };

        Because we_serialized_the_object = () =>
            Result = Serializer.Serialize("http://foo.com", Subject);

        It should_serialize_the_default_param = () =>
            Result.ShouldHaveParam("Param", Subject.Param);

        It should_serialize_the_param_with_custom_key = () =>
            Result.ShouldHaveParam("k", 10);

        It should_serialize_the_complex_type = () =>
            Result.ShouldHaveParam("t", "b");
    }

    public class When_I_deserialize_a_url
    {
        static Sample Result;
        static Uri Subject;
        static UrlSerializer Serializer;
     
        Establish context = () =>
        {
            Subject = new Uri("http://foo.com?param=foo&k=100&t=f");
            Serializer = new UrlSerializer();
        };

        Because we_des = () =>
            Result = Serializer.Deserialize<Sample>(Subject);

        It should_serialize_the_default_param = () =>
            Result.Param.ShouldEqual("foo");

        It should_serialize_the_param_with_custom_key = () =>
            Result.RequiredWithKey.ShouldEqual(100);

        It should_serialize_the_complex_type = () =>
            Result.Type.ShouldEqual(ComplexEnum.Foo);
    }
}