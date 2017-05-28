using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Core.Model.Network.Node.DataModel;
using Core.Model.Network.WebMethod.Repository;
using Newtonsoft.Json;

namespace Core.Model.Network.Node.Repository
{
	public class NodeRepository : INodeRepository
	{
		private readonly IWebMethodRepository _webMethodRepository;

		public NodeRepository(IWebMethodRepository web_method_repository)
		{
			_webMethodRepository = web_method_repository;
		}

		public bool Ping(DataModel.Node node, int timeout = 5000)
		{
			try
			{
				return _webMethodRepository.SendRequest<bool>($"{node.NetworkAddress.URI}:{node.NetworkAddress.Port}/{GetType().Name.Replace("Repository", "")}/{nameof(Ping)}" , timeout);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
