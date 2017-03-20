using System.Collections.Generic;

namespace Core.Model.Headers.Base
{
	/// <summary>
	/// Базоывй заголовок.
	/// </summary>
	public class Header
	{
		public virtual IEnumerable<Owner> Owners { get; set; }

	}
}

