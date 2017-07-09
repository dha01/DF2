﻿using Core.Model.CodeExecution.DataModel.Headers.Base;

namespace Core.Model.CodeExecution.DataModel.Bodies.Base
{
	public class ContainerBase : IContainer
	{
		public virtual InvokeHeader Header { get; set; }

		public string Token => Header.Token;
	}
}
