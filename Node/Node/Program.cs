using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.OpenInterfaces.Node;
using Core.Model.OpenInterfaces.Node.DataModel;
using Core.Model.OpenInterfaces.Node.Repository;
using Core.Model.OpenInterfaces.WebMethod.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Node.Static;


namespace Node
{
	
	public class StartupWithGreeting
	{
		public StartupWithGreeting(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
		/*	loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();
			*/
			app.UseMvc();
		}
	}
	
	public interface IGreeting
	{
		string Greet(string name);
	}

	public class MorningGreeting : IGreeting
	{
		public string Greet(string name)
		{
			return $"Good morning, {name}!";
		}
	}

	public class Person
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Age { get; set; }
	}

	class Program
	{
		static async Task MainAsync()
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://localhost:4000");

				/*   JsonConvert.SerializeObject(new {id = 125});
				   var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
				   {
					   new KeyValuePair<string, string>("id", "125")
				   });*/
				var json = JsonConvert.SerializeObject(new Person
				{
					Age = 10,
					FirstName = "a",
					LastName = "b"
				});
				var data = new StringContent(content: json,
					encoding: Encoding.UTF8,
					mediaType: "application/json");


				var result = await client.PostAsync("/Values/GetValue", data);
				string resultContent = await result.Content.ReadAsStringAsync();
				Console.WriteLine(resultContent);
			}
		}

		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntryAsync(Dns.GetHostName()).Result;
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("Local IP Address Not Found!");
		}
		public static string GetLocalIp()
		{
			return Dns.GetHostEntryAsync(Dns.GetHostName()).Result.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
		}
		static void Main(string[] args)
		{
			try
			{
				/*

				Task.Factory.StartNew(() =>
				{
					while (true)
					{
						Thread.Sleep(5000);
						MainAsync();
					}
				});*/


				var newtwork_address = new NetworkAddress() { URI = $"http://{GetLocalIp()}:{5000}", IPv4 = GetLocalIp(), Port = 5000 };

				StaticVariables.NodeServer = new NodeServer(new ServerService(newtwork_address), new NodeRepository(new WebMethodRepository()));

				/*var host = new WebHostBuilder()
					.UseKestrel()
					.UseContentRoot(Directory.GetCurrentDirectory())
					.UseIISIntegration()
					.UseStartup<Startup>()
					.UseUrls("http://localhost:4000")
					.Build();


				host.Run();*/
				StaticVariables.NodeServer.Run();
				Console.WriteLine("Running demo with Kestrel.");

				while (true)
				{
					var r = Console.ReadLine();
					if (r != null && r.Equals("end"))
					{
						break;
					}

					/*
					 
					Console.Read();
					StaticVariables.NodeServer.Stop();
					Console.WriteLine("Stop demo with Kestrel.");
					Console.Read();
					StaticVariables.NodeServer.Run();
					Console.WriteLine("Running demo with Kestrel.");
					Console.Read();
					StaticVariables.NodeServer.Stop();
					Console.WriteLine("Stop demo with Kestrel.");
					Console.Read();*/
				}



				/*
				var config = new ConfigurationBuilder()
				   // .AddCommandLine(args)
					.Build();

				var builder = new WebHostBuilder()
					.UseContentRoot(Directory.GetCurrentDirectory())
					.UseConfiguration(config)
					.UseStartup<StartupWithGreeting>()
					.ConfigureServices(service =>
					{
						service.AddSingleton<IGreeting, MorningGreeting>();
					})
					.UseKestrel(options =>
					{
						if (config["threadCount"] != null)
						{
							options.ThreadCount = int.Parse(config["threadCount"]);
						}
					})
					.UseUrls("http://localhost:5000");

				var host = builder.Build();
				host.Run();*/

				//   return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.Read();
		}
		
		private static object WebHostBuilder()
		{
			throw new NotImplementedException();
		}
	}
}