using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	/// <summary>
	/// Функция.
	/// </summary>
	public class Function : Base.ContainerBase, IFunction
	{

		/// <summary>
		/// Количество входных параметровю
		/// </summary>
		public int InputDataCount { get; set; }
	}
}

