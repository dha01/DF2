using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Node.Controllers
{
    public class ValuesController : Controller
    {
		public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

		[HttpPost]
		public string GetValue([FromBody]Person person)
        {
            return "value!";
		}
	}
}
