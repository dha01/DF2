﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Headers.Base;

namespace Core.Model.Bodies.Base
{
	public interface IContainer
	{
		InvokeHeader Header { get; set; }
	}
}
