using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/shoudongapi")]
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ShoudongAPI:Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            
            return new string[] { Assembly.GetExecutingAssembly().Location };
        }

    }
}

