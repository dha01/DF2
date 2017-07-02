using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.CodeExecution.DataModel.Bodies.Base;
using Core.Model.CodeExecution.DataModel.Headers.Base;

namespace Core.Model.CodeExecution.Repository
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
		protected ConcurrentDictionary<string, T_conteiner> _items;
		protected ConcurrentDictionary<string, T_header> _itemHeaders;

		protected ConcurrentDictionary<string, List<Action<T_header>>> _subscribes;

		//private Dictionary<T_header, List<Action<T_header>>> _subscribes;
		protected List<Action<T_header>> _unionSubscribe;


		//private ConcurrentList<> 

		public ContainerRepositoryBase()
		{
			_items = new ConcurrentDictionary<string, T_conteiner>();
			_itemHeaders = new ConcurrentDictionary<string, T_header>();
			_subscribes = new ConcurrentDictionary<string, List<Action<T_header>>>();
			_unionSubscribe = new List<Action<T_header>>();
		}

		protected virtual void AddConteiner(T_conteiner conteiner)
		{
			var key = string.Join("/", conteiner.Header.CallStack);
			if (_items.ContainsKey(key))
			{
				_items[key].Header.AddOwners(conteiner.Header.Owners);
			}
			else
			{
				_items[key] = conteiner;
			}
		}

		protected virtual void AddHeader(T_header header)
		{
			var key = string.Join("/", header.CallStack);
			if (_itemHeaders.ContainsKey(key))
			{
				_itemHeaders[key].AddOwners(header.Owners);
			}
			else
			{
				_itemHeaders[key] = header;
			}
		}

		public virtual void Add(IEnumerable<T_conteiner> conteiners, bool send_subsctibers = true)
		{
			// AddRange(conteiners);
			//_itemHeaders.AddRange(conteiners.Select(x => (T_header)x.Header));

			
			foreach (var conteiner in conteiners)
			{
				var key = string.Join("/", conteiner.Header.CallStack);
				AddConteiner(conteiner);
				AddHeader((T_header)conteiner.Header);

				//if (send_subsctibers)
				//{
					List<Action<T_header>> actions;
					_subscribes.TryRemove(key, out actions);
					if (actions != null)
					{
						Parallel.Invoke(actions.Select(x => new Action(() => { x.Invoke((T_header) conteiner.Header); })).ToArray());
					}

					Parallel.Invoke(_unionSubscribe.Select(x => new Action(() => { x.Invoke((T_header)conteiner.Header); })).ToArray());
				//}
				//Console.WriteLine(string.Format("ContainerRepositoryBase Add Callstack={0}", string.Join("/", conteiner.Header.CallStack)));
			}
		}

		public virtual IEnumerable<T> Get<T>(IEnumerable<T_header> headers) where T : T_conteiner
		{
			return Get(headers).Cast<T>();
		}

		public virtual IEnumerable<T_conteiner> Get(IEnumerable<T_header> headers)
		{
			var item = default(T_conteiner);
			return
				from header in headers
				where _items.TryGetValue(header.Token, out item)
				select item;
		}

		public virtual IEnumerable<T_conteiner> Get(params string[] tokens)
		{
			var item = default(T_conteiner);
			return
				from token in tokens
				where _items.TryGetValue(token, out item)
				select item;
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

		protected virtual bool IsItemExists(string key)
		{
			return _items.ContainsKey(key);
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

					if (IsItemExists(key))
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

