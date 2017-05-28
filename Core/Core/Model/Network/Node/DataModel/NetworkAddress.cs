using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Model.Network.Node.DataModel
{
	public class NetworkAddress
	{
		public IPAddress IPv4 { get; set; }
		public IPAddress IPv6 { get; set; }
		public string URI { get; set; }

		public int Port { get; set; }
	}
}
