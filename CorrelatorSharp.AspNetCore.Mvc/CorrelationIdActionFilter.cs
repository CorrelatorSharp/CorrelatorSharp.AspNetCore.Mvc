using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    public class CorrelationIdActionFilter : IAsyncActionFilter
    {
        private static readonly string CorrelationIdHttpHeader = Headers.CorrelationId;

        public bool AllowMultiple => false;

        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            string correlationId = null;

            var headers = actionContext.HttpContext.Request?.Headers;
            if (headers != null && headers.TryGetValue(CorrelationIdHttpHeader, out var correlationHeaderValue))
            {
                correlationId = correlationHeaderValue;
            }

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            using (var scope = ActivityScope.New(null, correlationId))
            {
                var actionResult = await next.Invoke();

                actionResult.HttpContext.Response.Headers.Add(CorrelationIdHttpHeader, scope.Id);
            }
        }
    }
}
