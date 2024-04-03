using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializebleDictionary<Key, Value> : Dictionary<Key, Value>, ISerializationCallbackReceiver
{
	[Serializable]
	public struct SerializableKeyValuePair<TKey, TValue>
	{
		public SerializableKeyValuePair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}
		public TKey key;
		public TValue value;
	}
	[SerializeField] List<SerializableKeyValuePair<Key, Value>> serialize;

	public void OnBeforeSerialize()
	{
		serialize = this.Select(pair => new SerializableKeyValuePair<Key, Value>(pair.Key, pair.Value)).ToList();
	}

	public void OnAfterDeserialize()
	{
		Clear();
		foreach (var keyValuePair in serialize)
			Add(keyValuePair.key, keyValuePair.value);
	}
}

[Serializable]
public class SerializebleHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
{
	[SerializeField] List<T> serialize;

	public void OnBeforeSerialize()
	{
		serialize = new List<T>(this);
	}

	public void OnAfterDeserialize()
	{
		Clear();
		foreach (var value in serialize)
			Add(value);
	}
}

[Serializable]
public struct SerializebleNullable<T> where T : struct
{
	[SerializeField] T value;
	[SerializeField] bool hasValue;

	public T Value
	{
		get
		{
			if (HasValue == false)
				return default;
			return value;
		}
	}
	public bool HasValue => hasValue;

	public SerializebleNullable(T value)
	{
		this.value = value;
		hasValue = true;
	}

	public static implicit operator SerializebleNullable<T>(T value) => new(value);
	public static implicit operator SerializebleNullable<T>(T? value) => value.HasValue ? new SerializebleNullable<T>(value.Value) : new SerializebleNullable<T>();
	public static implicit operator T?(SerializebleNullable<T> value) => value.HasValue ? value.Value : null;
	public override string ToString() => hasValue ? value.ToString() : string.Empty;
}