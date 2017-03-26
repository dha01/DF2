using System.Collections.Generic;
using System.Linq;

namespace Core.Model.Headers.Base
{
	/// <summary>
	/// Базоывй заголовок.
	/// </summary>
	public class Header
	{
		public virtual List<Owner> Owners { get; set; }

		public void AddOwners(IEnumerable<Owner> owners)
		{
			var new_owners = owners.Where(x => Owners.All(y => !y.IpAddress.GetAddressBytes().Equals(x.IpAddress.GetAddressBytes())));
			Owners.AddRange(new_owners);
		}

	}
}

