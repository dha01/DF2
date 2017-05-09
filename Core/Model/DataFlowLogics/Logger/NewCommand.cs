using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;

namespace Core.Model.Commands.Logger
{
	public class NewCommand
	{
		public DateTime DateTime { get; set; }
		public Command Command { get; set; }
	}
}
