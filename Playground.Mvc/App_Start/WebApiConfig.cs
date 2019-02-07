using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Playground.Mvc
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("Items API", "api/items/{id}", new { controller = "scopeitems", id = RouteParameter.Optional });
            //configuration.Routes.MapHttpRoute("Items API", "api/items/{id}", new { controller = "asyncitems", id = RouteParameter.Optional });
            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}