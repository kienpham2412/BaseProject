using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Essential
{
    public interface IPoolInstance
    {
        public void Release();
    }
    
    public abstract class PoolInstance<T> : MonoBehaviour, IPoolInstance where T : Component
    {
        public abstract ObjectPool<T> Pool { protected get; set; }

        public abstract void Release();
    }

    public class Pool<T> where T : PoolInstance<T>
    {
        public T Prefab { get; private set; }
        private Transform container;
        public ObjectPool<T> ObjectPool { get; private set; }
        private Vector3 initialPosition;

        public Pool(T prefab, Transform container, Vector3 initialPosition, int defaultCapacity = 10, int maxSize = 20)
        {
            this.Prefab = prefab;
            this.container = container;
            this.initialPosition = initialPosition;
            ObjectPool = new ObjectPool<T>(CreatePoolItem, null, ReleasePoolItem, DestroyPoolItem, false, defaultCapacity,
                maxSize);
        }

        private T CreatePoolItem()
        {
            var instance = GameObject.Instantiate(Prefab, container);
            instance.Pool = ObjectPool;
            instance.transform.localPosition = initialPosition;
            instance.gameObject.SetActive(false);
            return instance;
        }

        protected virtual void ReleasePoolItem(PoolInstance<T> system)
        {
            system.transform.SetParent(container);
            system.transform.localPosition = initialPosition;
            system.gameObject.SetActive(false);
        }

        private void DestroyPoolItem(PoolInstance<T> system)
        {
            GameObject.Destroy(system.gameObject);
        }

        public IEnumerator DisposeRoutine()
        {
            while (ObjectPool.CountActive > 0)
            {
                yield return null;
            }
            
            ObjectPool.Dispose();
        }

        public T Get()
        {
            return ObjectPool.Get();
        }
    }
}