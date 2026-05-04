using System.Collections.Generic;
using Enemies;
using UnityEngine;

public abstract class ObjectPool<T> : MonoSingleton<ObjectPool<T>>
    where T : MonoBehaviour, IPoolable
{
    private T _prefab;
    private readonly Stack<T> _available = new Stack<T>();

    protected void InitializePool(T prefab, int initialSize)
    {
        _prefab = prefab;

        for (int i = 0; i < initialSize; i++)
        {
            CreateInstance();
        }
    }

    protected T GetFromPool()
    {
        if (_available.Count == 0)
        {
            CreateInstance();
        }

        T obj = _available.Pop();
        obj.gameObject.SetActive(true);
        obj.OnTakenFromPool();
        return obj;
    }

    protected void ReturnToPool(T obj)
    {
        obj.OnReturnedToPool();
        obj.gameObject.SetActive(false);
        _available.Push(obj);
    }

    private void CreateInstance()
    {
        T instance = Instantiate(_prefab, transform);
        instance.gameObject.SetActive(false);
        _available.Push(instance);
    }
}