using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Core.Model.Bodies.Functions;
using Core.Model.Commands.Build;
using Core.Model.Compiler.Build.DataModel;
using Core.Model.Compiler.Code;
using Core.Model.Headers.Functions;
using Core.Model.InstructionExecutionConveyor.Extractors;
using Math = SimpleMethods.Simple.Math;

namespace SimpleMethods.Control
{
	public class SimpleStatic
	{
		//public static 
	}

	public class Simple : ControlFunctionBase
	{
		public void If(Var<int> a, Var<int> b)
		{
		}

		public static int Main2(int a)
		{
			return 1;
		}

		public static ControlFunction Main
		{
			get
			{
				var s = new Simple();
				var builder = s.GetFunc();
				s._Main((Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData());
				return CommandBuilder.Build(nameof(Main), $"{typeof(Simple).Namespace}.{typeof(Simple).Name}".Split('.').ToList(), () => s.GetFunc());

			}
		}

		public static ControlFunctionHeader MainHeader => CommandBuilder.BuildHeader(nameof(Main), $"{typeof(Simple).Namespace}.{typeof(Simple).Name}".Split('.').ToList());

		public void _Main(Var<int> a, Var<int> b, Var<int> c, Var<int> d, Var<int> e, Var<int> f, Var<int> g, Var<int> h)
	    {

		    var x1 = Exec(Math.Sum, a, b);
		    var x2 = Exec(Math.Sum, c, d);
		    var x3 = Exec(Math.Sum, e, f);
		    var x4 = Exec(Math.Sum, g, h);


		    var y1 = Exec(Math.Sum, x1, x2);
		    var y2 = Exec(Math.Sum, x3, x4);

		    var z = Exec(Math.Sum, y1, y2);
		    var z2 = Exec(Math.Sum, z, cmd.Constant(-1));

			Return(z2);
	    }
	}
}
