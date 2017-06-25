using Core.Model.OpenInterfaces.Node.Repository;
using Core.Model.OpenInterfaces.WebMethod.Repository;

namespace Core.Model.OpenInterfaces.Node
{
	public static class NodeExtension
	{
		public static NodeRepository NodeRepository = new NodeRepository(new WebMethodRepository());

		public static bool Ping(this DataModel.Node self, int timeout)
		{
			return NodeRepository.Ping(self, timeout);
		}
		public static DataModel.Node AddNode(this DataModel.Node self, DataModel.Node node, int timeout = 5000)
		{
			return NodeRepository.AddNode(node, timeout);
		}

		public static DataModel.Node GetInfo(this DataModel.Node self, int timeout = 5000)
		{
			var result = NodeRepository.GetInfo(self, timeout);

			if (result != null)
			{
				self.NetworkAddress = result.NetworkAddress;
				self.Guid = result.Guid;
				self.Index = result.Index;
				self.WorkingСapacity = result.WorkingСapacity;
				self.ProxyNodes = result.ProxyNodes;
			}
			
			return self;
		}
	}
}
