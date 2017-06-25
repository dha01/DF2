using Core.Model.OpenInterfaces.Node;
using Core.Model.OpenInterfaces.Node.DataModel;
using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class ManualController : Controller
	{
		public ManualController()
		{
		}

		public Core.Model.OpenInterfaces.Node.DataModel.Node JoinToNode(string address)
		{
			return new Core.Model.OpenInterfaces.Node.DataModel.Node()
			{
				NetworkAddress = new NetworkAddress()
				{
					URI = address
				}
			}.AddNode(StaticVariables.NodeServer.GetInfo());
		}
	}
}
