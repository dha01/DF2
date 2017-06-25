using System;
using System.Collections.Generic;

namespace Core.Model.OpenInterfaces.Node.DataModel
{
	public class Node
	{
		public Guid Guid { get; set; }
		public List<int> Index { get; set; }

		public NetworkAddress NetworkAddress { get; set; }

		public WorkingСapacity WorkingСapacity { get; set; }

		public List<Node> ProxyNodes { get; set; }

	}
}
