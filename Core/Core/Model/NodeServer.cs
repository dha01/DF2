using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Network.Node.DataModel;
using Core.Model.Network.Node.Repository;
using Core.Model.Network.Server.Service;
using Newtonsoft.Json;

namespace Core.Model
{
	public class NodeServer
	{
		private INodeRepository _nodeRepository;
		private IServerService _serverService;



		public NodeServer(IServerService server_service, INodeRepository node_repository)
		{
			_nodeRepository = node_repository;
			_serverService = server_service;
			/*
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					Thread.Sleep(1000);

					var result =  node_repository.Ping(new Node() { NetworkAddress = new NetworkAddress() { URI = "http://192.168.1.4", Port = 5000} });
					Console.WriteLine(result);
				}
			});*/

		}

		public NetworkAddress GetNetworkAddress()
		{
			return _serverService.NetworkAddress;
		}

		public bool Ping()
		{
			Console.WriteLine("Pinged");
			return true;
		}

		public void Run()
		{
			Task.Factory.StartNew(() =>
			{
				_serverService.Run();
			});
		}
		public void Stop()
		{
			_serverService.Stop();
		}
	}
}
