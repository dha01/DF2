using System.Threading;
using Core.Model.CodeCompiler.Build.Attributes;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeCompiler.Code;

namespace SimpleMethods.Control
{

	public class Simple : ControlFunctionBase
	{
		//public static ControlFunctionHeader MainHeader => CommandBuilder.BuildHeader(nameof(Main), $"{typeof(Simple).Namespace}.{typeof(Simple).Name}".Split('.').ToList());

		[ControlFunction]
		public void Main(Var<int> a, Var<int> b, Var<int> c, Var<int> d, Var<int> e, Var<int> f, Var<int> g, Var<int> h)
		{

			var cond = a == 1;
			Return(Iif(cond, Const(2) + Const(2), Const(2) * Const(2)));

			var x1 = (a + b) * (b + c);
			Return(Iif(a == 1 | a == 2, Const(1), Exec<int>("MyFunction", a - 1) + Exec<int>("MyFunction", a - 2)));
			Return(Iif(a == 1, Const(1), Iif(a == 2, Const(1), Exec<int>("MyFunction", a - 1) + Exec<int>("MyFunction", a - 2))));
			var x2 = x1 * x1;
			var x3 = x1 + x2;

			var x4 = Exec<int>("Second", a - 1, b + 1);

			var result = Iif(x2 == 0, x2, x3);
			//Exec(BasicFunctions.Any, x2, x3);
			Return(Any(x2, x3));
			Return(result);
		}

		[ControlFunction]
		public void Second(Var<int> a, Var<int> b)
		{
			Return(a + b);
		}

		//public void MainFeature(Var<int> a, Var<int> b, Var<int> c, Var<int> d, Var<int> e, Var<int> f, Var<int> g, Var<int> h)
			//{

			//	/*var x1 = a + b;// Exec(Math.Sum, a, b);
			//    var x2 = Exec(Math.Sum, c, d);
			//    var x3 = Exec(Math.Sum, e, f);
			//    var x4 = Exec(Math.Sum, g, h);


			//    var y1 = Exec(Math.Sum, x1, x2);
			//    var y2 = Exec(Math.Sum, x3, x4);

			//    var z = Exec(Math.Sum, y1, y2);
			//    var z2 = Exec(Math.Sum, z, Const(-1));*/

			//	//	var x1 = ((a + b) + (c + d)) + ((e + f) + (g + h));
			//	//var x1 = ((a + b) + (c + d)) + ((e + f) + (g + h));
			//	//var z = Exec<int, int, int>(MainHeader, a, b);
			//	var x1 = (a + b) * (b + c);

			//	var x2 = x1 * x1;
			//	var x3 = x1 + x2;

			//	var result = x2 - x3 + 1;

			//	/*
			//	var y = Null();

			//	var if1 = a + b == 5;
			//	var if2 = a + b == 6;

			//	var ifr1 = !if1 & !if2;
			//	var ifr2 = if1 & !if2;
			//	var ifr3 = if2;
			//	var yr1 = Iif(ifr1, 6);
			//	var yr2 = Iif(ifr2, 5);
			//	var yr3 = Iif(ifr3, 7);
			//	var r = Any(yr1, yr2, yr3);
			//	Return(r);
			//	*/
			//	/*
			//	var y = Null();

			//	var if1 = a + b == 5;
			//	var y0 = Iif(if1, 5);
			//	var y1 = Iif(!if1, 6);

			//	var if2 = a + b == 6;
			//	var y2 = Iif(if2, 7);

			//	Return(Last(y0, y1, y2));
			//	*/

			//	//var y0 = Null();
			//	/*
			//	var y = Null<int>();
			//	If(a + b == 5); //var if1 = a + b == 5;
			//		y.Set(5);
			//	Else();
			//		y.Set(6);
			//	End();


			//	If(a + b == 6); //var if2 = a + b == 6;
			//		y.Set(7);
			//	End();
			//	*/
			//	Return(result);
			//}
		}
}
