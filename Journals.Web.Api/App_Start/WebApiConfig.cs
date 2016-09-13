using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Journals.Web.Api.Filters;
using Newtonsoft.Json.Serialization;

namespace Journals.Web.Api {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Filters.Add(new GenericExceptionFilter());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
            "GenericAPI",
                "api/generic/{controller}/{action}/{context}",
                new {
                    context = RouteParameter.Optional, }
                );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
