using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void EventCollectionEventHandler<T>(T item, bool isAdd);
interface IEventCollection<T>
{
	public event EventCollectionEventHandler<T> Event;
	public event Action Changed;
}

[Serializable]
public class EventDictionary<Key, Value> : IEventCollection<Value>, IEnumerable<KeyValuePair<Key, Value>>, ISerializationCallbackReceiver
{
	[SerializeField] SerializebleDictionary<Key, Value> serialize = new();

	public event EventCollectionEventHandler<Value> Event;
	public event Action Changed;

	public Value this[Key key] => serialize[key];
	public EventDictionary() { }

	public void OnBeforeSerialize() { }
	public void OnAfterDeserialize()
	{
		foreach (var pair in serialize)
			Event?.Invoke(pair.Value, true);
		Changed?.Invoke();
	}

	public void Add(Key key, Value value)
	{
		serialize.Add(key, value);
		Event?.Invoke(value, true);
		Changed?.Invoke();
	}

	public void Clear()
	{
		var prev = serialize.Values.ToList();
		serialize.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public bool Remove(Key key)
	{
		var value = serialize[key];
		if (serialize.Remove(key))
		{
			Event?.Invoke(value, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public void OnChanged() => Changed?.Invoke();
	public void OnEvent(Value value, bool isAdd) => Event?.Invoke(value, isAdd);
	public bool ContainsKey(Key key) => serialize.ContainsKey(key);
	public Value Get(Key key) => serialize.Get(key);
	public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() => serialize.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => serialize.GetEnumerator();
}


[Serializable]
public class EventHashSet<T> : IEventCollection<T>, IEnumerable<T>, ISerializationCallbackReceiver
{
	[SerializeField] SerializebleHashSet<T> serialize = new();

	public int Count => serialize.Count;
	public event EventCollectionEventHandler<T> Event;
	public event Action Changed;

	public EventHashSet() { }

	public void OnBeforeSerialize() { }
	public void OnAfterDeserialize()
	{
		foreach (var item in serialize)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public bool Add(T item)
	{
		if (serialize.Add(item))
		{
			Event?.Invoke(item, true);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		var prev = serialize.ToList();
		serialize.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public bool Remove(T item)
	{
		if (serialize.Remove(item))
		{
			Event?.Invoke(item, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public void OnChanged() => Changed?.Invoke();
	public void OnEvent(T item, bool isAdd) => Event?.Invoke(item, isAdd);
	public bool Contains(T item) => serialize.Contains(item);
	public IEnumerator<T> GetEnumerator() => serialize.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => serialize.GetEnumerator();
}

[Serializable]
public class EventList<T> : IEventCollection<T>, IEnumerable<T>, ISerializationCallbackReceiver
{
	[SerializeField] List<T> serialize = new();

	public T this[int index] => serialize[index];

	public int Count => serialize.Count;
	public event EventCollectionEventHandler<T> Event;
	public event Action Changed;

	public EventList() { }
	public EventList(IEnumerable<T> collection) { AddRange(collection); }

	public void OnBeforeSerialize() { }
	public void OnAfterDeserialize()
	{
		foreach (var item in serialize)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public void Add(T item)
	{
		serialize.Add(item);
		Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public void AddRange(IEnumerable<T> collection)
	{
		serialize.AddRange(collection);
		foreach (var item in collection)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public void Clear()
	{
		var prev = new List<T>(serialize);
		serialize.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public bool Remove(T item)
	{
		if (serialize.Remove(item))
		{
			Event?.Invoke(item, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		var item = serialize[index];
		serialize.RemoveAt(index);
		Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public void RemoveAll(Func<T, bool> condition)
	{
		var target = new List<T>(serialize.Where(condition));
		for (int i = target.Count - 1; i >= 0; i--)
		{
			var prev = target[i];
			serialize.Remove(prev);
			Event?.Invoke(prev, false);
		}
		Changed?.Invoke();
	}

	public void OnChanged() => Changed?.Invoke();
	public void OnEvent(T item, bool isAdd) => Event?.Invoke(item, isAdd);
	public void ForEach(Action<T> action) => serialize.ForEach(action);
	public bool Contains(T item) => serialize.Contains(item);
	public IEnumerator<T> GetEnumerator() => serialize.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => serialize.GetEnumerator();
}
