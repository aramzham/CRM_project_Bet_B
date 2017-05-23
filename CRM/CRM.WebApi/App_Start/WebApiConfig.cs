using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CRM.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes

            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //name: "Pagination",
            //routeTemplate: "api/{controller}/{start}/{numberOfRows}/{ascOrDesc}",
            //defaults: new
            //{
            //    start = RouteParameter.Optional,
            //    numberOfRows = RouteParameter.Optional,
            //    ascOrDesc = RouteParameter.Optional
            //},
            //constraints: new
            //{
            //    start = @"\d{0,2}", //To be sure that the parameters are numbers
            //    numberOfRows = @"\d{0,2}",
            //    ascOrDesc = @"^([Tt][Rr][Uu][Ee]|[Ff][Aa][Ll][Ss][Ee])$"
            //}
            //);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
