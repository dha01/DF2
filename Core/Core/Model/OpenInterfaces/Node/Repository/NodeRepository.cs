﻿using System;
using Core.Model.OpenInterfaces.WebMethod.Repository;

namespace Core.Model.OpenInterfaces.Node.Repository
{
	public class NodeRepository : INodeRepository
	{
		private readonly IWebMethodRepository _webMethodRepository;

		public NodeRepository(IWebMethodRepository web_method_repository)
		{
			_webMethodRepository = web_method_repository;
		}

		public DataModel.Node AddNode(DataModel.Node node, int timeout = 5000)
		{
			try
			{
				return _webMethodRepository.SendRequest<DataModel.Node>($"{node.NetworkAddress.URI}/{GetType().Name.Replace("Repository", "")}/{nameof(AddNode)}", timeout);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public DataModel.Node GetInfo(DataModel.Node node, int timeout = 5000)
		{
			try
			{
				return _webMethodRepository.SendRequest<DataModel.Node>($"{node.NetworkAddress.URI}/{GetType().Name.Replace("Repository", "")}/{nameof(GetInfo)}", timeout);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public bool Ping(DataModel.Node node, int timeout = 5000)
		{
			try
			{
				return _webMethodRepository.SendRequest<bool>($"{node.NetworkAddress.URI}/{GetType().Name.Replace("Repository", "")}/{nameof(Ping)}" , timeout);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
