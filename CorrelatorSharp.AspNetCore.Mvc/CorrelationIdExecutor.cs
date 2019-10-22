using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    /// <summary>
    /// Executes the CorrelatorSharp middleware on a given HttpContext.
    /// </summary>
    internal class CorrelationIdExecutor
    {
        private static readonly string CorrelationIdHttpHeader = Headers.CorrelationId;
        private static readonly string CorrelationParentIdHttpHeader = Headers.CorrelationParentId;
        private static readonly string CorrelationNameHttpHeader = Headers.CorrelationName;

        public async Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next)
        {
            string correlationScopeName = null;
            string correlationId = null;
            string parentCorrelationId = null;

            var headers = context.Request?.Headers;
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
                await InvokeWithParentScope(next, context, correlationScopeName, correlationId, parentCorrelationId);
            }
            else
            {
                await InvokeWithNewScope(next, context, correlationScopeName, correlationId);
            }
        }

        private static async Task InvokeWithNewScope(Func<HttpContext, Task> next, HttpContext context, string correlationScopeName, string correlationId)
        {
            using (var scope = ActivityScope.Create(correlationScopeName, correlationId))
            {
                context.Response.OnStarting(state =>
                {
                    if (state is ActivityScope activity)
                    {
                        context.Response.Headers.Add(CorrelationParentIdHttpHeader, activity.ParentId);
                        context.Response.Headers.Add(CorrelationIdHttpHeader, activity.Id);
                    }

                    return Task.CompletedTask;
                }, scope);

                await next.Invoke(context);
            }
        }

        private static async Task InvokeWithParentScope(Func<HttpContext, Task> next, HttpContext context, string correlationScopeName, string correlationId, string parentCorrelationId)
        {
            using (ActivityScope.Create(null, parentCorrelationId))
            using (var child = ActivityScope.Child(correlationScopeName, correlationId))
            {
                context.Response.OnStarting(state =>
                {
                    if (state is ActivityScope activity)
                    {
                        context.Response.Headers.Add(CorrelationParentIdHttpHeader, activity.ParentId);
                        context.Response.Headers.Add(CorrelationIdHttpHeader, activity.Id);
                    }

                    return Task.CompletedTask;
                }, child);

                await next.Invoke(context);
            }
        }

    }
}