using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.Network.Node.Repository;
using Core.Model.Network.WebMethod.Repository;

namespace Core.Model.Network.Node
{
	public static class NodeExtension
	{
		public static NodeRepository NodeRepository = new NodeRepository(new WebMethodRepository());

		public static bool Ping(this DataModel.Node node, int timeout)
		{
			return NodeRepository.Ping(node, timeout);
		}

		public static DataModel.Node GetInfo(this DataModel.Node node, int timeout = 5000)
		{
			var result = NodeRepository.GetInfo(node, timeout);

			if (result != null)
			{
				node.NetworkAddress = result.NetworkAddress;
				node.Guid = result.Guid;
				node.Index = result.Index;
				node.WorkingСapacity = result.WorkingСapacity;
				node.ProxyNodes = result.ProxyNodes;
			}
			
			return node;
		}
	}
}
