using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Compiler.Build.DataModel
{
    public class Var<T>
    {
		public int Id { get; set; }

		public static implicit operator T(Var<T> var)
	    {
		    return default(T);
		}
		
	    public static explicit operator Var<T>(int var)
	    {
		    return new Var<T>(){Id = var};
	    }
		
		public static implicit operator int(Var<T> var)
	    {
		    return var.Id;
	    }

		/*
	    public static Var<T> operator +(Var<T> x, Var<T> y)
	    {
		    return var.Id;
	    }*/
	}
}
