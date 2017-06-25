using System;

namespace Core.Model.OpenInterfaces.Node.DataModel
{
	public class WorkingСapacity
	{
		public bool IsOnline { get; set; }
		public int CheckCount { get; set; }
		public int FailCheckCount { get; set; }
		public DateTime? LastCheckDateTime { get; set; }


	}
}
