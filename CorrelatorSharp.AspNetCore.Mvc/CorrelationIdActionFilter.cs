using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    /// <inheritdoc />
    public class CorrelationIdActionFilter : IAsyncActionFilter
    {
        private static readonly CorrelationIdExecutor Executor = new CorrelationIdExecutor();

        /// <inheritdoc />
        public Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next) 
            => Executor.InvokeAsync(actionContext.HttpContext, _ => next());
    }
}
