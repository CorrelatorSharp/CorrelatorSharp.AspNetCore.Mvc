using Microsoft.AspNetCore.Builder;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    /// <summary>
    /// Application builder extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add middleware to HTTP pipeline to handle Correlation information.
        /// Please ensure that this is added early in your pipeline so other middleware is correlated
        /// and that the middleware in registered in the ServiceCollection using the
        /// AddCorrelatorSharpMiddleware on the IMvcBuilder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorrelatorSharpMiddleware(this IApplicationBuilder builder) 
            => builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
