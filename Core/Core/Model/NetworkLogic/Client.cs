using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Core.Model.NetworkLogic
{
	public class Client
	{
		// Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
		public Client(TcpClient Client)
		{
			//	Console.WriteLine("Connect");
			var networkStream = Client.GetStream();
			
			var stream = new StreamReader(networkStream);


			var bodyf = stream.ReadLine();
			while (bodyf.Length > 0)
			{
				//bodyf = stream.Re();
			}


			//var body = stream.ReadLine();
			/*var uri = body.Split(new []{' '})[1];

			var sp = uri.Split('?');
			var method = sp.Length > 0 ? uri.Split('?')[0] : "";
			var input = sp.Length > 1 ? uri.Split('?')?[1] : "";
			var inputs = string.IsNullOrWhiteSpace(input) ? null : input.Split('&')?.ToDictionary(key => key.Split('=')[0], value => value.Split('=')[1]);
			*/
			//clientStreamReader = new StreamReader(networkStream);

			// Код простой HTML-странички
			string Html = "<html><body><h1>It works!</h1></body></html>";
			// Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
			string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
			// Приведем строку к виду массива байт
			byte[] Buffer = Encoding.ASCII.GetBytes(Str);
			// Отправим его клиенту
			Client.GetStream().Write(Buffer, 0, Buffer.Length);
			// Закроем соединение
			Client.Dispose();
		}
	}
}
