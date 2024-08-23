using Scripts.ObjectPooling;
using UnityEngine;

[System.Serializable]
public class ObjectPoolData<T> where T : RecyclableObject
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _initialInstances = 15;

    public ObjectPool ToObjectPool(Transform parent)
    {
        ObjectPool objectPool = new ObjectPool(_prefab, parent);
        objectPool.Init(_initialInstances);
        return objectPool;
    }
}