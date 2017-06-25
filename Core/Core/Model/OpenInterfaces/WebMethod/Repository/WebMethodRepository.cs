using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Model.Network.WebMethod.Repository
{
    public class WebMethodRepository : IWebMethodRepository
	{
		public Task<HttpResponseMessage> SendRequest(HttpClient http_client, string uri, string input_json)
		{
			http_client.BaseAddress = new Uri(uri/*"http://localhost:4000"*/);
			var data = new StringContent(content: input_json,
				encoding: Encoding.UTF8,
				mediaType: "application/json");
			return http_client.PostAsync(http_client.BaseAddress.LocalPath, data);
		}
		/*
		public Task<string> SendRequest(string uri, string input_json, int timeout = -1, out bool is_success)
		{
			using (var client = new HttpClient())
			{
				var request = SendRequest(client, uri, input_json);
				
				request.Wait(timeout);
				if (request.IsCompleted)
				{
					return request.Result.Content.ReadAsStringAsync();
				}

				throw new NotImplementedException("Не удалось установить соединение");
			}
		}*/

		public TOut SendRequest<TOut>(string uri, string input, int timeout)
		{
			using (var client = new HttpClient())
			{
				var request = SendRequest(client, uri, input);
				var has_result = request.Wait(timeout);

				if (has_result)
				{
					var responce_json = request.Result.Content.ReadAsStringAsync().Result;
					var responce = JsonConvert.DeserializeObject<TOut>(responce_json);

					return responce;
				}
				throw new NotImplementedException("Не удалось получить ответ.");
			}
		}

		public TOut SendRequest<TOut>(string uri, int timeout = 1000)
		{
			return SendRequest<TOut>(uri, "", timeout);
		}

		public TOut SendRequest<TIn, TOut>(string uri, TIn input = null, int timeout = 1000)
			where TIn : class
		{
			var json = input == null ? "" : JsonConvert.SerializeObject(input);
			return SendRequest<TOut>(uri, json, timeout);
		}
	}
}
