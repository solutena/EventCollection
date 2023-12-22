using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface IEventCollection<T>
{
	public event Action<T, bool> Event;
	public event Action Changed;
}

[Serializable]
public class EventDictionary<Key, Value> : IEventCollection<Value>, IEnumerable<KeyValuePair<Key, Value>>, ISerializationCallbackReceiver
{
	[SerializeField] List<KeyValuePair<Key, Value>> serialize;
	Dictionary<Key, Value> dictionary = new Dictionary<Key, Value>();

	public event Action<Value, bool> Event;
	public event Action Changed;

	public Value this[Key key] => dictionary[key];
	public EventDictionary() { }

	public void OnBeforeSerialize()
	{
		serialize = dictionary.ToList();
	}

	public void OnAfterDeserialize()
	{
		dictionary = serialize.ToDictionary(pair => pair.Key, pair => pair.Value);
		foreach (var value in dictionary.Values)
			Event?.Invoke(value, true);
		Changed?.Invoke();
	}

	public void Add(Key key, Value value)
	{
		dictionary.Add(key, value);
		Event?.Invoke(value, true);
		Changed?.Invoke();
	}

	public void Clear()
	{
		var prev = new List<Value>(dictionary.Values);
		dictionary.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public bool Remove(Key key)
	{
		var value = dictionary[key];
		if (dictionary.Remove(key))
		{
			Event?.Invoke(value, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public bool ContainsKey(Key key) => dictionary.ContainsKey(key);
	public Value Get(Key key) => dictionary.Get(key);
	public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() => dictionary.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
}

[Serializable]
public class EventHashSet<T> : IEventCollection<T>, IEnumerable<T>, ISerializationCallbackReceiver
{
	[SerializeField] List<T> serialize;
	HashSet<T> hashSet = new HashSet<T>();

	public int Count => hashSet.Count;
	public event Action<T, bool> Event;
	public event Action Changed;

	public EventHashSet() { }

	public void OnBeforeSerialize()
	{
		serialize = hashSet.ToList();
	}

	public void OnAfterDeserialize()
	{
		hashSet = serialize.ToHashSet();
		foreach (var item in hashSet)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public bool Add(T item)
	{
		if (hashSet.Add(item))
		{
			Event?.Invoke(item, true);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		var prev = new List<T>(hashSet);
		hashSet.Clear();
		foreach (var item in prev)
			Event?.Invoke(item, false);
		Changed?.Invoke();
	}

	public bool Remove(T item)
	{
		if (hashSet.Remove(item))
		{
			Event?.Invoke(item, false);
			Changed?.Invoke();
			return true;
		}
		return false;
	}

	public bool Contains(T item) => hashSet.Contains(item);
	public IEnumerator<T> GetEnumerator() => hashSet.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => hashSet.GetEnumerator();
}

[Serializable]
public class EventList<T> : IEventCollection<T>, IEnumerable<T>, ISerializationCallbackReceiver
{
	[SerializeField] List<T> serialize = new List<T>();

	public T this[int index] => serialize[index];

	public int Count => serialize.Count;
	public event Action<T, bool> Event;
	public event Action Changed;

	public EventList() { }
	public EventList(IEnumerable<T> collection) { AddRange(collection); }

	public void OnBeforeSerialize() { }
	public void OnAfterDeserialize() { serialize.ForEach(item => Event?.Invoke(item, true)); Changed?.Invoke(); }
	public void OnChanged() => Changed?.Invoke();
	public void OnEvent(T item, bool isAdd) => Event?.Invoke(item, isAdd);

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

	public void ForEach(Action<T> action) => serialize.ForEach(action);
	public bool Contains(T item) => serialize.Contains(item);
	public IEnumerator<T> GetEnumerator() => serialize.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => serialize.GetEnumerator();
}