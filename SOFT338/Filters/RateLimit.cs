using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SOFT338.Filters
{
    public class RateLimit : ActionFilterAttribute
    {
        /// <summary>
        /// The maximum number of API requests allowed per minute.
        /// </summary>
        public const int MAX_REQS_PER_MINUTE = 60;

        /// <summary>
        /// Represents the cache entry key.
        /// </summary>
        private string key;

        /// <summary>
        /// Executed before any controller action. Checks if the client has exceeded their rate limit and returns 429 if they have.
        /// </summary>
        /// <param name="actionContext">The HttpActionContext used by the controller action.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // Client IP address is used as the cache key
            this.key = this.getClientIp();

            if (HttpRuntime.Cache[this.key] == null)
            {
                // Client doesn't have a limit set yet, add them to the cache
                HttpRuntime.Cache.Add(this.key,
                    MAX_REQS_PER_MINUTE - 1,
                    null,
                    DateTime.Now.AddSeconds(60),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null);
            }
            else
            {
                int requestsLeft = (int)HttpRuntime.Cache.Get(this.key);

                if (requestsLeft > 0)
                {
                    // The cache has no update method (lame) so we have to remove it and re-add
                    DateTime expiry = this.getCacheExpiry(this.key);

                    HttpRuntime.Cache.Remove(this.key);
                    HttpRuntime.Cache.Add(this.key,
                        requestsLeft - 1,
                        null,
                        expiry,
                        Cache.NoSlidingExpiration,
                        CacheItemPriority.Low,
                        null);
                }
                else
                {
                    // Client has exhausted their allowed number of requests
                    actionContext.Response = actionContext.Request.CreateResponse(
                        (HttpStatusCode)429,
                        new { Message = "API rate limit exceeded. Please sleep until the time returned in the X-RateLimit-Reset header." },
                        actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

                    // Add headers
                    actionContext.Response.Headers.Add("X-RateLimit-Limit", MAX_REQS_PER_MINUTE.ToString());
                    actionContext.Response.Headers.Add("X-RateLimit-Remaining", requestsLeft.ToString());
                    actionContext.Response.Headers.Add("X-RateLimit-Reset", this.getCacheExpiry(this.key).ToString("s"));
                }
            }

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Executed after any controller action. Simply used to add rate limit headers to the response.
        /// </summary>
        /// <param name="actionExecutedContext">The HttpActionExecutedContext given by the controller action.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // For this method to be executed the client has not hit the rate limit, all we need to do is add the headers
            if (actionExecutedContext.Response != null)
            {
                actionExecutedContext.Response.Headers.Add("X-RateLimit-Limit", MAX_REQS_PER_MINUTE.ToString());
                actionExecutedContext.Response.Headers.Add("X-RateLimit-Remaining", HttpRuntime.Cache.Get(this.key).ToString());
                actionExecutedContext.Response.Headers.Add("X-RateLimit-Reset", this.getCacheExpiry(this.key).ToString("s"));
            }

            base.OnActionExecuted(actionExecutedContext);
        }

        /// <summary>
        /// Uses reflection to get the expiry time of a cache entry.
        /// </summary>
        /// <param name="cacheKey">The key of the cache entry.</param>
        /// <returns>A DateTime object set to the expiry time of the cache entry.</returns>
        private DateTime getCacheExpiry(string cacheKey)
        {
            object cacheEntry = HttpRuntime.Cache.GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(HttpRuntime.Cache, new object[] { cacheKey, 1 });
            PropertyInfo expiresProperty = cacheEntry.GetType().GetProperty("UtcExpires", BindingFlags.NonPublic | BindingFlags.Instance);
            DateTime expiresValue = (DateTime)expiresProperty.GetValue(cacheEntry, null);

            return expiresValue;
        }

        /// <summary>
        /// Get the public IP address of the client.
        /// </summary>
        /// <returns>A string representing the IP address of the client.</returns>
        private string getClientIp()
        {
            string address = string.Empty;

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                // Client is being proxied
                address = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                address = HttpContext.Current.Request.UserHostAddress;
            }

            return address;
        }
    }
}