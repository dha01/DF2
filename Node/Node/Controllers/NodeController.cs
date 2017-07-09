using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Options;
using Node.Static;
using RazorLight;
using RazorLight.Extensions;

namespace Node.Controllers
{
	public class NodeController : Controller
	{
		public NodeController()
		{
		}

		public ActionResult GetInfo()
		{
			/*string templatePath = $@"{Directory.GetCurrentDirectory()}\Views\Node";
			IRazorLightEngine engine = EngineFactory.CreatePhysical(templatePath);

			// Strings and anonymous models
			string stringResult = engine.ParseString("Hello @Model.Name", new { Name = "John" });

			// Files and strong models
			string resultFromFile = engine.Parse("GetInfo.cshtml", StaticVariables.NodeServer.GetInfo());
			
			return resultFromFile;*/
			
			return View(StaticVariables.NodeServer.GetInfo());
		}

		public Core.Model.OpenInterfaces.Node.DataModel.Node AddNode(string address)
		{
			return StaticVariables.NodeServer.AddNode(address);
		}

		public List<Core.Model.OpenInterfaces.Node.DataModel.Node> GetNodes()
		{
			return StaticVariables.NodeServer.GetNodes();
		}

		public bool Ping()
		{
			return StaticVariables.NodeServer.Ping();
		}

		[HttpPost]
		public string GetValue([FromBody]Person person)
		{
			return "value!";
		}

		public ActionResult A()
		{
			return View("Index");
		}

		public ActionResult State()
		{
			return View();
		}
	}
}
