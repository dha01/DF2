using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;

namespace Core.Model.Computing
{
	public interface ICommandService
	{
		/// <summary>
		/// Создает команду из заголовка. Если данных недостаточно, то возвращает <code>null</code>.
		/// </summary>
		/// <param name="command_header"></param>
		/// <param name="not_ready_data"></param>
		/// <returns></returns>
		Command CreateCommand(CommandHeader command_header, out Tuple<FunctionHeader, List<DataCellHeader>> not_ready_data);

		/// <summary>
		/// Проверяет наличие всех данных для выполнения команды.
		/// Если все данные доступны, то отправляет на исполнение.
		/// Если какие либо данные отсутствуют, то запрашивает их у других узлов.
		/// </summary>
		/// <param name="command_headers"></param>
		void PrepareAndInvokeCommands(IEnumerable<CommandHeader> command_headers, Action<Command> invoke_method);
	}
}
