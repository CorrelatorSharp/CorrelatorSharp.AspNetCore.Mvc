using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    public class CorrelationIdActionFilter : IAsyncActionFilter
    {
        private static readonly string CorrelationIdHttpHeader = Headers.CorrelationId;
        private static readonly string CorrelationParentIdHttpHeader = Headers.CorrelationParentId;
        private static readonly string CorrelationNameHttpHeader = Headers.CorrelationName;

        public bool AllowMultiple 
            => false;

        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            string correlationScopeName = null;
            string correlationId = null;
            string parentCorrelationId = null;

            var headers = actionContext.HttpContext.Request?.Headers;
            if (headers != null)
            {
                if (headers.TryGetValue(CorrelationIdHttpHeader, out var correlationHeaderValue))
                {
                    correlationId = correlationHeaderValue;
                }

                if (headers.TryGetValue(CorrelationParentIdHttpHeader, out var correlationParentHeaderValue))
                {
                    parentCorrelationId = correlationParentHeaderValue;
                }

                if (headers.TryGetValue(CorrelationNameHttpHeader, out var correlationNameHeaderValue))
                {
                    correlationScopeName = correlationNameHeaderValue;
                }
            }

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrWhiteSpace(parentCorrelationId) == false)
            {
                await InvokeWithParentScope(next, correlationScopeName, correlationId, parentCorrelationId);
            }
            else
            {
                await InvokeWithNewScope(next, correlationScopeName, correlationId);
            }
        }

        private static async Task InvokeWithNewScope(ActionExecutionDelegate next, string correlationScopeName, string correlationId)
        {
            using (var scope = ActivityScope.Create(correlationScopeName, correlationId))
            {
                var actionResult = await next.Invoke();

                actionResult.HttpContext.Response.Headers.Add(CorrelationIdHttpHeader, scope.Id);
            }
        }

        private static async Task InvokeWithParentScope(ActionExecutionDelegate next, string correlationScopeName, string correlationId, string parentCorrelationId)
        {
            using (var parent = ActivityScope.Create(null, parentCorrelationId))
            using (var child = ActivityScope.Child(correlationScopeName, correlationId))
            {
                var actionResult = await next.Invoke();

                actionResult.HttpContext.Response.Headers.Add(CorrelationParentIdHttpHeader, parent.Id);
                actionResult.HttpContext.Response.Headers.Add(CorrelationIdHttpHeader, child.Id);
            }
        }
    }
}
