using SOFT338.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SOFT338
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Use attributes defined on controller actions for routing
            config.MapHttpAttributeRoutes();

            // Register global filters
            config.Filters.Add(new RateLimit());
        }
    }
}
