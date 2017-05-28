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
	}
}
