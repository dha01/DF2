using System;
using System.Collections.Generic;
using Core.Model.OpenInterfaces.Node.DataModel;

namespace Core.Model.NetworkLogic.Net.DataModel
{
	public class Ticket
	{


		public Guid NetGuid { get; set; }

		public Demension Demension { get; set; }

		public Node ThisNode { get; set; }
		public List<Node> Proxy { get; set; }
	}
}
