using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.Network.Node.DataModel;

namespace Core.Model.Network.Server.Service
{
    public interface IServerService
    {
		NetworkAddress NetworkAddress { get; set; }
		void Run();
		void Stop();
    }
}
