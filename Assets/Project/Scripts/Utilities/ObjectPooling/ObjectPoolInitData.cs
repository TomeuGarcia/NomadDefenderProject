using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.ObjectPooling
{

    [System.Serializable]
    public class ObjectPoolInitData<T> where T : RecyclableObject
    {
        [SerializeField] private T _prefab;
        [SerializeField] private int _initialInstances = 15;

        public ObjectPool MakeInitializedObjectPool(Transform parent)
        {
            ObjectPool objectPool = new ObjectPool(_prefab, parent);
            objectPool.Init(_initialInstances);
            return objectPool;
        }
    }

}