using Microsoft.AspNetCore.Mvc;

namespace CorrelatorSharp.AspNetCore.Mvc
{
    /// <summary>
    /// MVC options extensions.
    /// </summary>
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// Add the CorrelatorSharp middleware to the MVC filter collection.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static MvcOptions AddCorrelatorSharpFilter(this MvcOptions options)
        {
            options.Filters.Add(new CorrelationIdActionFilter());
            return options;
        }
    }
}