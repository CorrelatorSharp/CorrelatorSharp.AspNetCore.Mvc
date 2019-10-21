using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    /// <inheritdoc />
    public class CorrelationIdMiddleware : IMiddleware
    {
        private static readonly CorrelationIdExecutor Executor = new CorrelationIdExecutor();

        /// <inheritdoc />
        public Task InvokeAsync(HttpContext context, RequestDelegate next) 
            => Executor.InvokeAsync(context, httpContext => next(httpContext));
    }
}
