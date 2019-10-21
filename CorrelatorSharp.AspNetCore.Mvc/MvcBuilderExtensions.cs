using Microsoft.Extensions.DependencyInjection;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    /// <summary>
    /// MVC services builder extensions.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Add the CorrelatorSharp middleware to the DI for MVC.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddCorrelatorSharpMiddleware(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<CorrelationIdMiddleware>();
            return builder;
        }
    }
}
