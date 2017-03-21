using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Base;
using Core.Model.Headers.Base;

namespace Core.Model.Repository
{
	/// <summary>
	/// Базовый репозиторий контейнеров.
	/// </summary>
	/// <typeparam name="T_conteiner"></typeparam>
	/// <typeparam name="T_header"></typeparam>
	public class ContainerRepositoryBase<T_conteiner, T_header> : IContainerRepository<T_conteiner, T_header> 
		where T_conteiner : IContainer 
		where T_header : InvokeHeader
	{
		private ConcurrentDictionary<string, T_conteiner> _items;
		private ConcurrentDictionary<string, T_header> _itemHeaders;

		private ConcurrentDictionary<string,  List<Action<T_header>>> _subscribes;

		//private Dictionary<T_header, List<Action<T_header>>> _subscribes;
		private List<Action<T_header>> _unionSubscribe;


		//private ConcurrentList<> 

		public ContainerRepositoryBase()
		{
			_items = new ConcurrentDictionary<string, T_conteiner>();
			_itemHeaders = new ConcurrentDictionary<string, T_header>();
			_subscribes = new ConcurrentDictionary<string, List<Action<T_header>>>();
			_unionSubscribe = new List<Action<T_header>>();
		}
		
		public virtual void Add(IEnumerable<T_conteiner> conteiners)
		{
			// AddRange(conteiners);
			//_itemHeaders.AddRange(conteiners.Select(x => (T_header)x.Header));

			
			foreach (var conteiner in conteiners)
			{
				var key = string.Join("/", conteiner.Header.CallStack);
				_items[key] = conteiner;
				_itemHeaders[key] = (T_header)conteiner.Header;

				List<Action<T_header>> actions;
				_subscribes.TryRemove(key, out actions);
				if (actions != null)
				{
					Parallel.Invoke(actions.Select(x => new Action(() => { x.Invoke((T_header) conteiner.Header); })).ToArray());
				}
				Parallel.Invoke(_unionSubscribe.Select(x => new Action(() => { x.Invoke((T_header)conteiner.Header); })).ToArray());

				//Console.WriteLine(string.Format("ContainerRepositoryBase Add Callstack={0}", string.Join("/", conteiner.Header.CallStack)));
			}
		}

		public virtual IEnumerable<T_conteiner> Get(IEnumerable<T_header> headers)
		{
			var list = new List<T_conteiner>();
			foreach (var header in headers)
			{
				var key = string.Join("/", header.CallStack);
				if (_items.ContainsKey(key))
				{
					list.Add(_items[key]);
				}
			}
			return list;
		}

		public virtual void AddHeaders(IEnumerable<T_header> headers)
		{

			foreach (var header in headers)
			{
				//var key = _itemHeaders.FirstOrDefault(x => x.Equals(header));
				var key = string.Join("/", header.CallStack);
				_itemHeaders[key] = header;
			}
		}

		public virtual void Subscribe(IEnumerable<T_header> headers, Action<T_header> callback)
		{
			if (headers == null)
			{
				_unionSubscribe.Add(callback);
			}
			else
			{
				foreach (var header in headers)
				{
					var key = string.Join("/", header.CallStack);

					if (_items.ContainsKey(key))
					{
						var local_header = header;
						Parallel.Invoke(() => { callback.Invoke(local_header); });
						continue;
					}

					if (_subscribes.ContainsKey(key))
					{
						_subscribes[key].Add(callback);
					}
					else
					{
						_subscribes[key] = new List<Action<T_header>>() { callback };
					}
				}
			}
		}
	}
}

