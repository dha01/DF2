using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;

namespace Core.Model.Repository
{
	public class DataCellRepository : ContainerRepositoryBase<DataCell, DataCellHeader>, IDataCellRepository
	{
	}
}
