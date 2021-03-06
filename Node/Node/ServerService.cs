﻿using System;
using System.IO;
using System.Threading;
using Core.Model.OpenInterfaces.Node.DataModel;
using Core.Model.OpenInterfaces.Server.Service;
using Microsoft.AspNetCore.Hosting;

namespace Node
{
	public class ServerService : IServerService
	{
		public NetworkAddress NetworkAddress { get; set; }

		private IWebHost _host { get; set; }

		public ServerService(NetworkAddress network_address)
		{
			NetworkAddress = network_address;
		}

		public void Run()
		{
			while (true)
			{
				try
				{
					_host = new WebHostBuilder()
						.UseKestrel()
						.UseContentRoot(Directory.GetCurrentDirectory())
						.UseIISIntegration()
						.UseStartup<Startup>()
						.UseUrls(new[] { /*$"{NetworkAddress.URI}:{NetworkAddress.Port}",*/ $"http://{NetworkAddress.IPv4}:{NetworkAddress.Port}" } /*"http://localhost:4000"*/)
						.Build();
					_host.Run();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);

					if (e.Message.Contains("port already in use") || e.Message.Contains("address already in use"))
					{
						NetworkAddress.Port++;
						Thread.Sleep(1000);
						continue;
					}
				}
				break;
			}
		}

		public void Stop()
		{
			_host.Dispose();
		}
	}
}
