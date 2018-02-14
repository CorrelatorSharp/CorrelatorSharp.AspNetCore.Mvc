using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CorrelatorSharp.AspNetCore.Mvc.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new [] { $"CorrelatorSharp.AspNetCore.Mvc - {ActivityScope.Current.Id}" };
        }
    }
}
