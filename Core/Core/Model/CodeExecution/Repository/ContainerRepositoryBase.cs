using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Core.Model.CodeExecution.DataModel;
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
		protected class Tree
		{
			public string Name { get; set; }
			public ConcurrentDictionary<string, T_conteiner> _values = new ConcurrentDictionary<string, T_conteiner>();
			public ConcurrentDictionary<string, Tree> _items = new ConcurrentDictionary<string, Tree>();

			public Tree(string lvl_name)
			{
				Name = lvl_name;
			}

			public T_conteiner Get(IEnumerable<string> path)
			{
				var part = path.First();

				if (path.Count() > 1)
				{
					if (_items.TryGetValue(part, out Tree tree))
					{
						var result = tree.Get(path.Skip(1));
						return result;
					}
				}
				else
				{
					if (_values.TryGetValue(part, out T_conteiner value))
					{
						return value;
					}
				}

				return default(T_conteiner);
			}

			public void Delete(IEnumerable<string> path)
			{
				var part = path.First();

				if (_items.TryGetValue(part, out Tree tree))
				{
					var count = path.Count();
					if (path.Count() > 1)
					{
						tree.Delete(path.Skip(1));
					}
					else
					{
						_items.TryRemove(part, out tree);
					}
				}
				if (_values.TryGetValue(part, out T_conteiner cont))
				{
					if (path.Count() > 1)
					{
						tree.Delete(path.Skip(1));
					}
					else
					{
						_values.TryRemove(part, out cont);
					}
				}
			}

			public void DeleteChilds(IEnumerable<string> path)
			{
				var part = path.First();
				Tree tree;
				T_conteiner cont;

				var count = path.Count();
				if (path.Count() > 1)
				{
					if (_items.TryGetValue(part, out tree))
					{
						tree.DeleteChilds(path.Skip(1));
					}
				}
				else
				{
					if (_items.TryGetValue(part, out tree))
					{
						tree._items.Clear();
						foreach (var key in _values.Keys.Where(x => !x.StartsWith("result")))
						{
							_values.TryRemove(key, out cont);
						}
					}
				}
			}

			public void Add(T_conteiner item, IEnumerable<string> path = null)
			{
				path = path ?? item.Token.ToEnumerable();

				var part = path.First();

				if (path.Count() > 1)
				{
					if (!_items.TryGetValue(part, out Tree tree))
					{
						tree = new Tree(part);
						if (!_items.TryAdd(part, tree))
						{
							if (!_items.TryGetValue(part, out tree))
							{
								var c = 6;
							}
						}
					}
					tree.Add(item, path.Skip(1));
				}
				else
				{
					//item.Header.Token = null;
					//item.Header.CallStack = null;
					if (!_values.TryAdd(part, item))
					{
						var c = 5;
					}
				}
			}
		}
		protected Tree _itemsTree = new Tree("root");

		//protected ConcurrentDictionary<string, T_conteiner> _items;
		//protected ConcurrentDictionary<string, T_header> _itemHeaders;

		protected ConcurrentDictionary<string, List<Action<T_header>>> _subscribes;

		//private Dictionary<T_header, List<Action<T_header>>> _subscribes;
		protected List<Action<T_header>> _unionSubscribe;


		//private ConcurrentList<> 

		public ContainerRepositoryBase()
		{
			//_items = new ConcurrentDictionary<string, T_conteiner>();
			//_itemHeaders = new ConcurrentDictionary<string, T_header>();
			_subscribes = new ConcurrentDictionary<string, List<Action<T_header>>>();
			_unionSubscribe = new List<Action<T_header>>();
		}

		protected virtual void AddConteiner(T_conteiner conteiner)
		{
			if (IsItemExists(conteiner.Token))
			{
				//_items[key].Header.AddOwners(conteiner.Header.Owners);
			}
			else
			{
				//_items[conteiner.Token] = conteiner;
				_itemsTree.Add(conteiner);
			}
		}

		protected virtual void AddHeader(T_header header)
		{
			/*if (_itemHeaders.ContainsKey(header.Token))
			{
				//_itemHeaders[key].AddOwners(header.Owners);
			}
			else
			{
				_itemHeaders[header.Token] = header;
			}*/
		}

		public virtual void Add(IEnumerable<T_conteiner> conteiners, bool send_subsctibers = true)
		{
			// AddRange(conteiners);
			//_itemHeaders.AddRange(conteiners.Select(x => (T_header)x.Header));

			
			foreach (var conteiner in conteiners)
			{
				AddConteiner(conteiner);
				AddHeader((T_header)conteiner.Header);

				//if (send_subsctibers)
				//{
					List<Action<T_header>> actions;
					_subscribes.TryGetValue(conteiner.Token, out actions);
					if (actions != null)
					{
						Parallel.Invoke(actions.Select(x => new Action(() =>
						{
							x.Invoke((T_header) conteiner.Header);
							//_subscribes.TryRemove(conteiner.Token, out actions);
						})).ToArray());
					}

					Parallel.Invoke(_unionSubscribe.Select(x => new Action(() =>
					{
						x.Invoke((T_header)conteiner.Header);
						//_subscribes.TryRemove(conteiner.Token, out actions);
					})).ToArray());
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
			return from header in headers
				select _itemsTree.Get(header.Token.ToEnumerable());
		}

		public virtual IEnumerable<T_conteiner> Get(params string[] tokens)
		{
			var item = default(T_conteiner);

			var first = (from token in tokens
				select _itemsTree.Get(token.Split('/'))).ToList();
			
		/*	var second = 
				(from token in tokens
				select _items.TryGetValue(token, out item) ? item : default(T_conteiner)).ToList();

			if (first.Count(x => x != null) != second.Count(x => x != null))
			{
				// TODO: что то иногда не совпадает.
				var t = 5;
			}*/

			return first;
		}


		public virtual void Delete(params string[] tokens)
		{
			foreach (var token in tokens)
			{
				_itemsTree.Delete(token.Trim('/').Split('/'));
				//_items.TryRemove(token, out T_conteiner container);
				//_itemHeaders.TryRemove(token, out T_header container_header);
			}
		}

		public virtual void DeleteChilds(params string[] tokens)
		{
			foreach (var token in tokens)
			{
				_itemsTree.DeleteChilds(token.Trim('/').Split('/'));
			}
		}

		public virtual void DeleteStartWith(params string[] tokens)
		{
			//var deleted_keys = _items.Keys.ToList().Where(x => tokens.Any(y => x.StartsWith(y)) && !_subscribes.ContainsKey(x));
			Delete(tokens);
		}

		public virtual void AddHeaders(IEnumerable<T_header> headers)
		{

			foreach (var header in headers)
			{
				//var key = _itemHeaders.FirstOrDefault(x => x.Equals(header));
				//_itemHeaders[header.Token] = header;
			}
		}

		protected virtual bool IsItemExists(string key)
		{
			//return _items.ContainsKey(key);
			return _itemsTree.Get(key.Split('/')) != null;
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
					if (IsItemExists(header.Token))
					{
						var local_header = header;
						Parallel.Invoke(() => { callback.Invoke(local_header); });
						continue;
					}

					if (_subscribes.ContainsKey(header.Token))
					{
						_subscribes[header.Token].Add(callback);
					}
					else
					{
						_subscribes[header.Token] = new List<Action<T_header>>() { callback };
					}
				}
			}
		}


		public ConteinerRepositoryInfo GetConteinerRepositoryInfo()
		{
			return new ConteinerRepositoryInfo
			{
				//ContainerCount = _items.Count,
				//ContainerHeaderCount = _itemHeaders.Count
			};
		}
	}
}

