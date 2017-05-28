using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Model.Network
{
	public class Server2
	{
		TcpListener Listener; // Объект, принимающий TCP-клиентов

		public Server2()
		{

		}


		public static void ClientThread(Object StateInfo)
		{
			new Client((TcpClient)StateInfo);
		}

		async public void Run(int port)
		{
			// Создаем "слушателя" для указанного порта
			Listener = new TcpListener(IPAddress.Any, port);
			Listener.Start(); // Запускаем его

			// В бесконечном цикле
			while (true)
			{
				TcpClient tcp_client = await Listener.AcceptTcpClientAsync();
				ClientThread(tcp_client);
			}
		}


		// Остановка сервера
		~Server2()
		{
			// Если "слушатель" был создан
			// Остановим его
			Listener?.Stop();
		}
	}
}
