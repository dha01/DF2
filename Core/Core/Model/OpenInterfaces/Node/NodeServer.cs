using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Commands.Build;
using Core.Model.Computing;
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

		// TODO: сделать интерфейсом.
		private ComputingCore _computingCore;

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

			_computingCore = ComputingCore.InitComputingCore();
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

		private static int index = 0;
		public string InvokeCode(string code)
		{
			index++;
			//var computing_core = ComputingCore.InitComputingCore();
			//	computing_core.AddAssembly(@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods\bin\Debug\netcoreapp1.1\SimpleMethods.dll");

			var assembly = CommandBuilder.CreateFunctionFromSourceCode(@"
using Core.Model.Compiler.Build.DataModel;
using Core.Model.Compiler.Code;

namespace CustomNamespace" + index.ToString() + @"
{
	public class CustomClass : ControlFunctionBase
	{
		" + code + @"
	}
}");
			_computingCore.AddAssembly(assembly);
			var result = _computingCore.Exec($"CustomNamespace{index}.CustomClass.Main", 1, 2, 3);
			result.Wait(10000);
			return JsonConvert.SerializeObject(result.Result.Data);
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
