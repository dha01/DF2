using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Network.Node;
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

		/// <summary>
		/// Информация о других известных узлах.
		/// </summary>
		private List<Node> _nodes = new List<Node>();

		/// <summary>
		/// Информация об этом узле.
		/// </summary>
		private Node _info;

		public NodeServer(IServerService server_service, INodeRepository node_repository)
		{
			_nodeRepository = node_repository;
			_serverService = server_service;

			_info = new Node()
			{
				Guid = Guid.NewGuid(),
				NetworkAddress = server_service.NetworkAddress,
				WorkingСapacity = new WorkingСapacity()
				{
					CheckCount = -1,
					FailCheckCount = 0,
					IsOnline = true,
					LastCheckDateTime = null
				},
				ProxyNodes = new List<Node>(),
				Index = new List<int>() { 1 }
			};
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

		#region Web methods

		public Node AddNode(string address)
		{
			var node = new Node()
			{
				NetworkAddress = new NetworkAddress()
				{
					URI = address
				}
			}.GetInfo();

			_nodes.Add(node);
			return node;
		}

		public List<Node> GetNodes()
		{
			return _nodes;
		}

		public Node GetInfo()
		{
			return _info;
		}

		public bool Ping()
		{
			Console.WriteLine("Pinged");
			return true;
		}

		#endregion

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
