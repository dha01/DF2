using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Model.Network.Node.DataModel
{
	public class NetworkAddress
	{
		public int Port { get; set; }
		public string URI { get; set; }
		public string IPv4 { get; set; }
		public string IPv6 { get; set; }
	}
}
