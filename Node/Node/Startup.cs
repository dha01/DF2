using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Node.Static;

namespace Node
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				//.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
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
			/*loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "rol",
					template: "{controller}/{action}");
			});*/

			loggerFactory.AddConsole(LogLevel.Debug);
			loggerFactory.AddDebug(LogLevel.Debug);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "rol",
					template: "{controller}/{action}");
			});

			app.UseWebSockets();

			#region AcceptWebSocket
			app.Use(async (context, next) =>
			{
				if (context.Request.Path == "/ws")
				{
					if (context.WebSockets.IsWebSocketRequest)
					{
						WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
						await Echo(context, webSocket);
					}
					else
					{
						context.Response.StatusCode = 400;
					}
				}
				else
				{
					await next();
				}

			});
			#endregion
			app.UseFileServer();
		}
		#region Echo
		private async Task Echo(HttpContext context, WebSocket webSocket)
		{
			var buffer = new byte[1024 * 4];
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					Thread.Sleep(2000);
					var arr = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(StaticVariables.NodeServer.GetComputingCoreInfo()));
					webSocket.SendAsync(new ArraySegment<byte>(arr, 0, arr.Length), WebSocketMessageType.Text, true, CancellationToken.None);
					
				}
			});

			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
		#endregion
	}
}
