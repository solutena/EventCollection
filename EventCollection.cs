using System;
using System.Collections.Generic;
using UnityEngine;

interface IEventCollection<T>
{
	public event Action<T, bool> Event;
	public Action Changed { get; set; }
}

[Serializable]
public class EventList<T> : List<T>, IEventCollection<T>, ISerializationCallbackReceiver
{
	[SerializeField] List<T> serialize;

	public event Action<T, bool> Event;
	public Action Changed { get; set; }

	public EventList() { }
	public EventList(IEnumerable<T> collection) { AddRange(collection); }

	public void OnBeforeSerialize() => serialize = new List<T>(this);
	public void OnAfterDeserialize() => serialize.ForEach(item => Add(item));

	public new void Add(T item)
	{
		Debug.Log("Add");
		base.Add(item);
		Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public new void AddRange(IEnumerable<T> collection)
	{
		Debug.Log("AddRange");
		base.AddRange(collection);
		foreach (var item in collection)
			Event?.Invoke(item, true);
		Changed?.Invoke();
	}
	public new void Insert(int index, T item)
	{
		Debug.Log("Insert");
		base.Insert(index, item);
		Event?.Invoke(item, true);
		Changed?.Invoke();
	}

	public new void InsertRange(int index, IEnumerable<T> collection)
	{
		Debug.Log("InsertRange");
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