using System.Collections.Generic;
using UnityEngine;

namespace Managment.Pooling
{
    public class MonoPool<TPool, T> : MonoSingleton<MonoPool<TPool,T>> 
        where TPool : MonoPool<TPool,T>
        where T: MonoBehaviour, IPoolable
    {
        [SerializeField] T prefab;
        [SerializeField] int initialSize = 10;
        [SerializeField] int increaseSize = 10;
    
        private readonly Stack<T> _available = new();

        protected void Awake()
        {
            IncreasePool(initialSize);
        }

        public T Get()
        {
            if (_available.Count < 1)
            {
                IncreasePool(increaseSize);
            }
            var pooledObject = _available.Pop();
        
        

            // C# Knows this object implements IPoolable!
            pooledObject.OnTakenFromPool();
            // C# Knows this object is a MonoBehaviour!
            pooledObject.gameObject.SetActive(true);
        
            return pooledObject;
        }
  
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _available.Push(obj);
        }
    
        private void IncreasePool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                var instance = Instantiate(prefab, parent: this.transform);
                Return(instance);
            }
        }
    }
}