using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;

namespace Core.Model.Repository
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
				var key = string.Join("/", conteiner.Header.CallStack);

				if (conteiner is ControlFunction)
				{
					var control_function = (ControlFunction)conteiner;

					int i = 0;
					foreach (var constant in control_function.Constants)
					{
						var callstack = new List<string>();
						callstack.Add(conteiner.GetHeader<FunctionHeader>().CallstackToString("."));
						callstack.Add(string.Format("const_{0}", i));

						_dataCellRepository.Add(new []{ new DataCell()
						{
							Header = new DataCellHeader()
							{
								CallStack = callstack,
								HasValue = new Dictionary<Owner, bool>(),
								Owners = new List<Owner>(),
							},HasValue = true,
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
				_subscribes.TryRemove(key, out actions);
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
