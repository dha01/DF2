using System;
using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel;

namespace Core.Model.CodeExecution.Repository
{

	public interface IContainerRepository<T_conteiner, T_header>
	{
		void Add(IEnumerable<T_conteiner> conteiner, bool send_subscribes = true);

		IEnumerable<T> Get<T>(IEnumerable<T_header> header) where T : T_conteiner;

		IEnumerable<T_conteiner> Get(IEnumerable<T_header> header);
		IEnumerable<T_conteiner> Get(params string[] tokens);
		void Delete(params string[] tokens);
		void DeleteStartWith(params string[] tokens);

		void AddHeaders(IEnumerable<T_header> header);

		void Subscribe(IEnumerable<T_header> header, Action<T_header> callback);


		ConteinerRepositoryInfo GetConteinerRepositoryInfo();

	}
}

