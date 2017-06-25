namespace Core.Model.CodeExecution.DataModel.Headers.Functions
{
	/// <summary>
	/// Заголовок функции C#.
	/// </summary>
	public class CSharpFunctionHeader : ComputingFunctionHeader
	{
		public virtual string FileName { get; set; }

		public virtual string FunctionName { get; set; }

		public virtual string Version { get; set; }

		public virtual string Namespace { get; set; }

	}
}

