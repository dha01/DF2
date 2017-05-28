using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Network.Net.DataModel
{
    public class Axis
    {
		public int Index { get; set; }
		public Node.DataModel.Node Prev { get; set; }
	    public Node.DataModel.Node Next { get; set; }

		public int Length { get; set; }
	}
}
