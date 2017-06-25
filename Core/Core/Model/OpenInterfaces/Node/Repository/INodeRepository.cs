using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Network.Node.Repository
{
    public interface INodeRepository
    {
	    bool Ping(DataModel.Node node, int timeout = 5000);
    }
}
