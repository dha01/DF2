using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Commands.Logger
{
	public class FunctionCall
	{
		public FunctionCall Parent { get; set; } 
		
		public DateTime CallDateTime { get; set; }
		public List<string> CallStack { get; set; }

		public int Lvl { get; set; }
		public string LvlName { get; set; }

		public List<string> InputDataNames { get; set; }
		public string OutputDataName { get; set; }
		public string FunctionName { get; set; }

		public List<FunctionCall> Childs { get; set; }
	}
}
