using System.Net;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace FoundationCore.Web.Business
{
    public class RedirectLowerCaseRule : IRule
    {
        public int StatusCode { get; } = (int)HttpStatusCode.MovedPermanently;

        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            var path = context.HttpContext.Request.Path;
            var host = context.HttpContext.Request.Host;

            if (path.Value != null && (path.HasValue && path.Value.Any(char.IsUpper) || host.HasValue && host.Value.Any(char.IsUpper)))
            {
                if (!path.StartsWithSegments("/episerver") && !path.StartsWithSegments("/api") && !path.StartsWithSegments("/util"))
                {
                    var response = context.HttpContext.Response;
                    response.StatusCode = StatusCode;
                    response.Headers[HeaderNames.Location] =
                        (request.Scheme + "://" + host.Value + request.PathBase.Value + request.Path.Value).ToLower() +
                        request.QueryString;
                    context.Result = RuleResult.EndResponse;
                }
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
