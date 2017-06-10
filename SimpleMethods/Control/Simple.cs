using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
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
		public Var<T> Null<T>()
		{
			return new Var<T>(cmd);
		}

		public void If(Var<bool> a)
		{
		}

		public Var<T> Iif<T>(Var<bool> a, Var<T> val)
		{
			throw new NotImplementedException();
		}

		public Var<T> Iif<T>(Var<bool> a, T val)
		{
			return Iif(a, new Var<T>(cmd) {Id = cmd.Constant(val) });
		}

		public void Else()
		{
		}

		public void End()
		{
		}

		public static int Main2(int a)
		{
			return 1;
		}
		/*
		public static ControlFunction Main
		{
			get
			{
				var s = new Simple();
				var builder = s.GetFunc();
				s._Main((Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData());
				return CommandBuilder.Build(nameof(Main), $"{typeof(Simple).Namespace}.{typeof(Simple).Name}".Split('.').ToList(), () => s.GetFunc());

			}
		}*/
		

		public static ControlFunctionHeader MainHeader => CommandBuilder.BuildHeader(nameof(Main), $"{typeof(Simple).Namespace}.{typeof(Simple).Name}".Split('.').ToList());

		public void Main(Var<int> a, Var<int> b, Var<int> c, Var<int> d, Var<int> e, Var<int> f, Var<int> g, Var<int> h)
		{

			/*var x1 = a + b;// Exec(Math.Sum, a, b);
		    var x2 = Exec(Math.Sum, c, d);
		    var x3 = Exec(Math.Sum, e, f);
		    var x4 = Exec(Math.Sum, g, h);


		    var y1 = Exec(Math.Sum, x1, x2);
		    var y2 = Exec(Math.Sum, x3, x4);

		    var z = Exec(Math.Sum, y1, y2);
		    var z2 = Exec(Math.Sum, z, Const(-1));*/

			//	var x1 = ((a + b) + (c + d)) + ((e + f) + (g + h));
			//var x1 = ((a + b) + (c + d)) + ((e + f) + (g + h));
			//var z = Exec<int, int, int>(MainHeader, a, b);
			var x1 = (a + b) * f / h * g;


			/*
			var y = Null();

			var if1 = a + b == 5;
			var if2 = a + b == 6;

			var ifr1 = !if1 & !if2;
			var ifr2 = if1 & !if2;
			var ifr3 = if2;
			var yr1 = Iif(ifr1, 6);
			var yr2 = Iif(ifr2, 5);
			var yr3 = Iif(ifr3, 7);
			var r = Any(yr1, yr2, yr3);
			Return(r);
			*/
			/*
			var y = Null();

			var if1 = a + b == 5;
			var y0 = Iif(if1, 5);
			var y1 = Iif(!if1, 6);

			var if2 = a + b == 6;
			var y2 = Iif(if2, 7);

			Return(Last(y0, y1, y2));
			*/

			//var y0 = Null();
			/*
			var y = Null<int>();
			If(a + b == 5); //var if1 = a + b == 5;
				y.Set(5);
			Else();
				y.Set(6);
			End();

			
			If(a + b == 6); //var if2 = a + b == 6;
				y.Set(7);
			End();
			*/
			Return(x1);
	    }
	}
}
