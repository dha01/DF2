using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;

namespace Core.Model.Compiler.Code
{
    public static class BasicFunctions
    {
	    public static BasicFunction Sum = new BasicFunction()
	    {
		    Id = 1,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "+",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "+" },
			    Id = 1
		    }
	    };

	    public static BasicFunction Sub = new BasicFunction()
	    {
		    Id = 2,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "-",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "-" },
			    Id = 2
		    }
	    };

	    public static BasicFunction Mul = new BasicFunction()
	    {
		    Id = 3,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "*",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "*" },
			    Id = 3
		    }
	    };

	    public static BasicFunction Div = new BasicFunction()
	    {
		    Id = 4,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "/",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "/" },
			    Id = 4
		    }
	    };

	    public static BasicFunction Equal = new BasicFunction()
	    {
		    Id = 5,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "==",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "==" },
			    Id = 5
		    }
	    };

	    public static BasicFunction NotEqual = new BasicFunction()
	    {
		    Id = 6,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "!=",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "!==" },
			    Id = 6
		    }
	    };

	    public static BasicFunction Not = new BasicFunction()
	    {
		    Id = 7,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "!",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "!" },
			    Id = 7
		    }
	    };

	    public static BasicFunction And = new BasicFunction()
	    {
		    Id = 7,
		    Header = new BasicFunctionHeader()
		    {
			    Name = "&",
			    Owners = new List<Owner>(),
			    CallStack = new List<string>() { "BasicFunctions", "&" },
			    Id = 7
		    }
	    };
	}
}
