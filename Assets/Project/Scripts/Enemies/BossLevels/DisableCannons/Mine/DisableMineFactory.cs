using System;
using Scripts.ObjectPooling;
using UnityEngine;

public class DisableMineFactory : MonoBehaviour
{
    [SerializeField] private ObjectPoolInitData<DisableMine> _objectPoolInitData;
    private ObjectPool _pool;
    

    public void Init()
    {
        _pool = _objectPoolInitData.MakeInitializedObjectPool(transform);
    }

    public void Create(Vector3 position)
    {
        _pool.Spawn<DisableMine>(position, Quaternion.identity);
    }
}