using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Headers.Base;

namespace Core.Model.Repository
{
	public class ContainerRepositoryBase<T_conteiner, T_header> : IContainerRepository<T_conteiner, T_header> 
		where T_conteiner : IContainer 
		where T_header : InvokeHeader
	{
		private List<T_conteiner> _items;
		private List<T_header> _itemHeaders;

		private Dictionary<T_header, Action<T_header>> _subscribes; 

		public ContainerRepositoryBase()
		{
			_items = new List<T_conteiner>();
			_itemHeaders = new List<T_header>();
			_subscribes = new Dictionary<T_header, Action<T_header>>();
		}
		
		public virtual void Add(IEnumerable<T_conteiner> conteiners)
		{
			_items.AddRange(conteiners);
			_itemHeaders.AddRange(conteiners.Select(x => (T_header)x.Header));

			foreach (var conteiner in conteiners)
			{
				var key = _subscribes.Keys.FirstOrDefault(x => x.Equals(conteiner.Header));
				if (key != null)
				{
					_subscribes[key].Invoke((T_header)conteiner.Header);
					_subscribes.Remove(key);
				}
			}
		}

		public virtual IEnumerable<T_conteiner> Get(IEnumerable<T_header> headers)
		{
			var list = new List<T_conteiner>();
			foreach (var header in headers)
			{
				var key = _items.FirstOrDefault(x => x.Header.Equals(header));
				if (key != null)
				{
					list.Add(key);
				}
			}
			return list;
		}

		public virtual void AddHeaders(IEnumerable<T_header> header)
		{
			_itemHeaders.AddRange(header);
		}

		public virtual void Subscribe(IEnumerable<T_header> headers, Action<T_header> callback)
		{
			foreach (var header in headers)
			{
				var key = _subscribes.Keys.FirstOrDefault(x => x.Equals(header));
				if (key != null)
				{
					_subscribes[key] += callback;
				}
				else
				{
					_subscribes.Add(header, callback);
				}
			}
		}
	}
}

