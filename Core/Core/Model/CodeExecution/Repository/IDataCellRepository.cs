using System;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Headers.Data;

namespace Core.Model.CodeExecution.Repository
{
	

	public interface IDataCellRepository  : IContainerRepository<DataCell, DataCellHeader>
	{

		void CreateDublicate(params Tuple<string, string>[] dublicates);
	}
}

