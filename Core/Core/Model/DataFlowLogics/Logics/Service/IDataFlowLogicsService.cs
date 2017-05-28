﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Headers.Commands;

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
	}
}