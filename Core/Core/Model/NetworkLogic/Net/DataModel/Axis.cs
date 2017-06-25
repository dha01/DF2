using Core.Model.OpenInterfaces.Node.DataModel;

namespace Core.Model.NetworkLogic.Net.DataModel
{
	public class Axis
	{
		public int Index { get; set; }
		public Node Prev { get; set; }
		public Node Next { get; set; }

		public int Length { get; set; }
	}
}
