using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.DataFlowLogics.Logics.DataModel;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	public interface IDataFlowLogicsService
	{
		void AddNewCommandHeader(IEnumerable<CommandHeader> command_headers);
		
		/// <summary>
		/// Для уже существующих заголовков команд добаляет в список владельцев новых владельцев команды.
		/// Новые команды отправляет на подготовку к исполнению.
		/// </summary>
		/// <param name="command_header"></param>
		void AddNewCommandHeader(CommandHeader command_header);


		/// <summary>
		/// Событие, которое выполняется при завершения исполнения команды на исполнителе.
		/// </summary>
		/// <param name="command">Команда, выполнение которой было завершено.</param>
		void OnExecutedCommand(Command command);

		/// <summary>
		/// Возвращает количество команд в каждой из очередей.
		/// </summary>
		/// <returns></returns>
		StateQueuesInfo GetStateQueuesInfo();

		void OnDataReady(string data_cell_token);
	}
}
