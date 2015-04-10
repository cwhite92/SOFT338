using SOFT338.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace SOFT338.Filters
{
    public class BasicAuth : Attribute, IAuthenticationFilter
    {
        // All of our endpoints reside under the same realm, hence it's okay to statically define it here
        private string realm = "api";
        public bool AllowMultiple { get { return false; } }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;

            // If a HTTP basic Authorization header is provided
            if (request.Headers.Authorization != null &&
                request.Headers.Authorization.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Convert from base64
                    Encoding encoding = Encoding.GetEncoding("utf-8");
                    string credentials = encoding.GetString(Convert.FromBase64String(request.Headers.Authorization.Parameter));

                    string[] parts = credentials.Split(':');
                    string email = parts[0];
                    string password = parts[1];

                    using (ApiDbContext db = new ApiDbContext())
                    {
                        // Find a user with this email
                        User user = db.Users.Where(u => u.Email == email).FirstOrDefault();

                        if (user != null)
                        {
                            // Verify their password
                            if (user.CheckPassword(password))
                            {
                                GenericIdentity identity = new GenericIdentity(user.Id.ToString());
                                GenericPrincipal principal = new GenericPrincipal(identity, new string[] { "User" });
                                context.Principal = principal;
                            }
                            else
                            {
                                // Incorrect password
                                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                            }
                        }
                        else
                        {
                            // User doesn't exist
                            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                        }
                    }
                }
                catch (FormatException)
                {
                    // Invalid base64 passed through Authorization header
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            else
            {
                // No Authorization header set
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result, this.realm);
            return Task.FromResult(0);
        }
    }

    public class ResultWithChallenge : IHttpActionResult
    {
        private readonly IHttpActionResult next;
        private readonly string realm;

        public ResultWithChallenge(IHttpActionResult next, string realm)
        {
            this.next = next;
            this.realm = realm;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = await next.ExecuteAsync(cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", this.realm));
            }

            return result;
        }
    }
}