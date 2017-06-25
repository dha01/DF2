namespace Core.Model.OpenInterfaces.Node.Repository
{
	public interface INodeRepository
	{
		bool Ping(DataModel.Node node, int timeout = 5000);
	}
}
