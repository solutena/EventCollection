using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[Serializable]
	public struct SerializableKeyValuePair
	{
		public SerializableKeyValuePair(TKey key, TValue value) { this.key = key; this.value = value; }
		public TKey key;
		public TValue value;
	}

	public delegate void EventDelegate(TKey key, TValue value, bool isAdd);

	[SerializeField] List<SerializableKeyValuePair> serialize;

	public Action Changed { get; set; }
	public event EventDelegate Event;

	public EventDictionary() : base() { }
	public EventDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
	public EventDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }

	public void OnBeforeSerialize()
	{
		serialize = new List<SerializableKeyValuePair>();
		foreach (var item in this)
			serialize.Add(new SerializableKeyValuePair(item.Key, item.Value));
	}

	public void OnAfterDeserialize()
	{
		base.Clear();
		foreach (var item in serialize)
		{
			base.Add(item.key, item.value);
			Event?.Invoke(item.key, item.value, true);
		}
		Changed?.Invoke();
	}

	public new void Add(TKey key, TValue value)
	{
		base.Add(key, value);
		Event?.Invoke(key, value, true);
		Changed?.Invoke();
	}

	public new bool TryAdd(TKey key, TValue value)
	{
		if (base.TryAdd(key, value))
		{
			Event?.Invoke(key, value, true);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public new bool Remove(TKey key)
	{
		if (base.Remove(key, out TValue value))
		{
			Event?.Invoke(key, value, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public new bool Remove(TKey key, out TValue value)
	{
		if (base.Remove(key, out value))
		{
			Event?.Invoke(key, value, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public new void Clear()
	{
		var prev = new List<KeyValuePair<TKey, TValue>>(this);
		base.Clear();
		foreach (var item in prev)
			Event?.Invoke(item.Key, item.Value, false);
		Changed?.Invoke();
	}
}

[Serializable]
public class EventHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
{
	public delegate void EventDelegate(T item, bool isAdd);

	[SerializeField] List<T> serialize;

	public Action Changed { get; set; }
	public event EventDelegate Event;

	public EventHashSet() : base() { }
	public EventHashSet(IEnumerable<T> collection) : base(collection) { }

	public void OnBeforeSerialize() => serialize = new List<T>(this);
	public void OnAfterDeserialize()
	{
		base.Clear();
		foreach (var item in serialize)
		{
			base.Add(item);
			Event?.Invoke(item, true);
		}
		Changed?.Invoke();
	}

	public new bool Add(T item)
	{
		if (base.Add(item))
		{
			Event?.Invoke(item, true);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public new bool Remove(T item)
	{
		if (base.Remove(item))
		{
			Event?.Invoke(item, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public new int RemoveWhere(Predicate<T> match)
	{
		int numRemoved = 0;
		var item = GetEnumerator();
		while (item.MoveNext())
		{
			if (match(item.Current))
			{
				Event?.Invoke(item.Current, false);
				numRemoved++;
			}
		}
		if (0 < numRemoved)
			Changed?.Invoke();
		return numRemoved;
	}

	public new void Clear()
	{
		var prev = new List<T>(this);
		base.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}
}

[Serializable]
public class EventList<T> : List<T>, ISerializationCallbackReceiver
{
	public delegate void EventDelegate(T item, bool isAdd);

	[SerializeField] List<T> serialize = new();

	public Action Changed { get; set; }
	public event EventDelegate Event;

	public EventList() : base() { }
	public EventList(IEnumerable<T> collection) : base(collection) { Changed?.Invoke(); }

	public void OnBeforeSerialize() => serialize = new List<T>(this);
	public void OnAfterDeserialize() { base.Clear(); AddRange(serialize); }

	public new void Add(T item)
	{
		base.Add(item);
		Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public new void AddRange(IEnumerable<T> collection)
	{
		base.AddRange(collection);
		foreach (var item in collection)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public new void Insert(int index, T item)
	{
		base.Insert(index, item);
		Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public new void InsertRange(int index, IEnumerable<T> collection)
	{
		base.InsertRange(index, collection);
		foreach (var item in collection)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public new void Clear()
	{
		var prev = new List<T>(this);
		base.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public new bool Remove(T item)
	{
		if (base.Remove(item))
		{
			Event?.Invoke(item, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public new void RemoveAt(int index)
	{
		var item = this[index];
		base.RemoveAt(index);
		Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public new int RemoveAll(Predicate<T> match)
	{
		var target = new List<T>(FindAll(match));
		int count = base.RemoveAll(match);
		for (int i = target.Count - 1; i >= 0; i--)
		{
			var prev = target[i];
			base.Remove(prev);
			Event?.Invoke(prev, false);
		}
		Changed?.Invoke();
		return count;
	}

	public new void Reverse(int index, int count) { base.Reverse(index, count); Changed?.Invoke(); }
	public new void Reverse() { base.Reverse(); Changed?.Invoke(); }
	public new void Sort(Comparison<T> comparison) { base.Sort(comparison); Changed?.Invoke(); }
	public new void Sort(int index, int count, IComparer<T> comparer) { base.Sort(index, count, comparer); Changed?.Invoke(); }
	public new void Sort() { base.Sort(); Changed?.Invoke(); }
	public new void Sort(IComparer<T> comparer) { base.Sort(comparer); Changed?.Invoke(); }
}