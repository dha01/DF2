using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Network.Net.DataModel
{
    public class Ticket
    {


		public Guid NetGuid { get; set; }

		public Demension Demension { get; set; }

		public Node.DataModel.Node ThisNode { get; set; }
	    public List<Node.DataModel.Node> Proxy { get; set; }
	}
}
