using Core.Model.OpenInterfaces.Node.DataModel;

namespace Core.Model.OpenInterfaces.Server.Service
{
	public interface IServerService
	{
		NetworkAddress NetworkAddress { get; set; }
		void Run();
		void Stop();
	}
}
