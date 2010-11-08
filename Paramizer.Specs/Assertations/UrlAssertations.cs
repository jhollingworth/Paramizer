using System;
using System.Collections.Specialized;
using System.Web;
using Machine.Specifications;

namespace Paramizer.Specs.Assertations
{
    public static class UrlAssertations
    {
         public static void ShouldHaveParam(this Uri url, string key, object expectedValue)
         {
             var actualValue = HttpUtility.ParseQueryString(url.Query)[key];

             expectedValue.ToString().ShouldEqual(actualValue);
         }
    }
}