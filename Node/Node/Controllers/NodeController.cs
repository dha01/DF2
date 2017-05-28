using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Network.Node.DataModel;
using Core.Model.Network.Node.Repository;
using Core.Model.Network.WebMethod.Repository;
using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class NodeController : Controller
	{
		public NodeController()
		{
		}

		public NetworkAddress GetNetworkAddress()
		{
			return StaticVariables.NodeServer.GetNetworkAddress();
		}

		public bool AddNode(Core.Model.Network.Node.DataModel.Node node)
		{
			return StaticVariables.NodeServer.AddNode(node);
		}

		public List<Core.Model.Network.Node.DataModel.Node> GetNodes()
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
