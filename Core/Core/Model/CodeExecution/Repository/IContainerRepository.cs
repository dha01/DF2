using System;
using System.Collections.Generic;

namespace Core.Model.CodeExecution.Repository
{

	public interface IContainerRepository<T_conteiner, T_header>
	{
		void Add(IEnumerable<T_conteiner> conteiner, bool send_subscribes = true);

		IEnumerable<T> Get<T>(IEnumerable<T_header> header) where T : T_conteiner;

		IEnumerable<T_conteiner> Get(IEnumerable<T_header> header);

		void AddHeaders(IEnumerable<T_header> header);

		void Subscribe(IEnumerable<T_header> header, Action<T_header> callback);

	}
}

