using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeExecution.Service.Computing;
using Core.Model.CodeExecution.Service.DataModel;
using Core.Model.DataFlowLogics.Logics.DataModel;
using Core.Model.OpenInterfaces.Node.DataModel;
using Core.Model.OpenInterfaces.Node.Repository;
using Core.Model.OpenInterfaces.Server.Service;
using Newtonsoft.Json;

namespace Core.Model.OpenInterfaces.Node
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
		private List<DataModel.Node> _nodes = new List<DataModel.Node>();

		/// <summary>
		/// Информация об этом узле.
		/// </summary>
		private DataModel.Node _info;

		public NodeServer(IServerService server_service, INodeRepository node_repository)
		{
			_nodeRepository = node_repository;
			_serverService = server_service;

			_info = new DataModel.Node()
			{
				Guid = Guid.NewGuid(),
				NetworkAddress = server_service.NetworkAddress,
				WorkingCapacity = new WorkingCapacity()
				{
					CheckCount = -1,
					FailCheckCount = 0,
					IsOnline = true,
					LastCheckDateTime = null
				},
				ProxyNodes = new List<DataModel.Node>(),
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

		public DataModel.Node AddNode(string address)
		{
			var node = new DataModel.Node()
			{
				NetworkAddress = new NetworkAddress()
				{
					URI = address
				}
			}.GetInfo();

			_nodes.Add(node);
			return node;
		}

		public List<DataModel.Node> GetNodes()
		{
			return _nodes;
		}

		public DataModel.Node GetInfo()
		{
			return _info;
		}

		public ComputingCoreInfo GetComputingCoreInfo()
		{
			return _computingCore.GetComputingCoreInfo();
		}

		public bool Ping()
		{
			Console.WriteLine("Pinged");
			return true;
		}

		private static int index = 0;
		public string InvokeCode(string code, string input)
		{
			index++;
			//var computing_core = ComputingCore.InitComputingCore();
			//	computing_core.AddAssembly(@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods\bin\Debug\netcoreapp1.1\SimpleMethods.dll");

			var assembly = CommandBuilder.CreateFunctionFromSourceCode(@"
using Core.Model.CodeCompiler.Build.Attributes;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeCompiler.Code;

namespace CustomNamespace" + index.ToString() + @"
{
	public class CustomClass : ControlFunctionBase
	{
		" + code + @"
	}
}");
			_computingCore.AddAssembly(assembly);
			var result = _computingCore.Exec($"CustomNamespace{index}.CustomClass.Main", input.Split(';').Select(x => (object)Convert.ToInt32(x)).ToArray());
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
