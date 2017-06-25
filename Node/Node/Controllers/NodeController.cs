using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class NodeController : Controller
	{
		public NodeController()
		{
		}

		public Core.Model.OpenInterfaces.Node.DataModel.Node GetInfo()
		{
			return StaticVariables.NodeServer.GetInfo();
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
	}
}
