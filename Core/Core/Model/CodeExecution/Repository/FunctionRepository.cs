using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.NetworkLogic;

namespace Core.Model.CodeExecution.Repository
{
	/// <summary>
	/// Репозиторий функций.
	/// </summary>
	public class FunctionRepository : ContainerRepositoryBase<Function, FunctionHeader>, IFunctionRepository
	{
		private IDataCellRepository _dataCellRepository;
		public FunctionRepository(IDataCellRepository data_cell_repository)
		{
			_dataCellRepository = data_cell_repository;
		}
		
		
		public override void Add(IEnumerable<Function> conteiners, bool send_subsctibers = true)
		{
			// AddRange(conteiners);
			//_itemHeaders.AddRange(conteiners.Select(x => (T_header)x.Header));


			foreach (var conteiner in conteiners)
			{
				if (conteiner is ControlFunction)
				{
					var control_function = (ControlFunction)conteiner;

					int i = 0;
					foreach (var constant in control_function.Constants)
					{
						_dataCellRepository.Add(new []{ new DataCell()
						{
							Header = new DataCellHeader()
							{
								Token = conteiner.Token.Next($"const_{i}")
							},
							HasValue = true,
							Data = constant
						} });
						i++;
					}
				}
				AddConteiner(conteiner);
				AddHeader((FunctionHeader)conteiner.Header);

				//if (send_subsctibers)
				//{
				List<Action<FunctionHeader>> actions;
				_subscribes.TryRemove(conteiner.Token, out actions);
				if (actions != null)
				{
					Parallel.Invoke(actions.Select(x => new Action(() => { x.Invoke((FunctionHeader)conteiner.Header); })).ToArray());
				}

				Parallel.Invoke(_unionSubscribe.Select(x => new Action(() => { x.Invoke((FunctionHeader)conteiner.Header); })).ToArray());
				//}
				//Console.WriteLine(string.Format("ContainerRepositoryBase Add Callstack={0}", string.Join("/", conteiner.Header.CallStack)));
			}
		}
	}
}
