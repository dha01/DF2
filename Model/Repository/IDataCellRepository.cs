using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Data;

namespace Core.Model.Repository
{
	

	public interface IDataCellRepository  : IContainerRepository<DataCell, DataCellHeader>
	{
	}
}

