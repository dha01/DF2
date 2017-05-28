using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Network.Node;
using Core.Model.Network.Node.DataModel;
using Core.Model.Network.Node.Repository;
using Core.Model.Network.WebMethod.Repository;
using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class ManualController : Controller
	{
		public ManualController()
		{
		}

		public Core.Model.Network.Node.DataModel.Node JoinToNode(string address)
		{
			return new Core.Model.Network.Node.DataModel.Node()
			{
				NetworkAddress = new NetworkAddress()
				{
					URI = address
				}
			}.AddNode(StaticVariables.NodeServer.GetInfo());
		}
	}
}
