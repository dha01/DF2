using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Headers.Base;
using Core.Model.Headers.Data;

namespace Core.Model.Repository
{

	public interface IContainerRepository<T_conteiner, T_header>
	{
		void Add(IEnumerable<T_conteiner> conteiner, bool send_subscribes = true);

		IEnumerable<T_conteiner> Get(IEnumerable<T_header> header);

		void AddHeaders(IEnumerable<T_header> header);

		void Subscribe(IEnumerable<T_header> header, Action<T_header> callback);

	}
}

