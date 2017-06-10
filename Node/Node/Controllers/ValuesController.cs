using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using SimpleMethods.CustomMethods;

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
			//var x = SimpleMethods.CustomMethods.Math
			//var x = Math2.Sum;
			return "value!";
		}
	}
}
