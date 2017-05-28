using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Network.Node.DataModel
{
    public class WorkingСapacity
    {
		public bool IsOnline { get; set; }
	    public int CheckCount { get; set; }
	    public int FailCheckCount { get; set; }
	    public DateTime LastCheckDateTime { get; set; }


	}
}
