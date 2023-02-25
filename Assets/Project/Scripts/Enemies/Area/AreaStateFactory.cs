using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaStateFactory : MonoBehaviour
{
    [System.Serializable]
    private struct AreaTypeToPool
    {
        public AreaFunctionality.AreaType type;
        public Pool pool;
    }
    private static AreaStateFactory instance;

    [SerializeField] private AreaTypeToPool[] areasToPool;
    private Dictionary<AreaFunctionality.AreaType, Pool> sortedAreas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        TDGameManager.OnEndGameResetPools += ResetPools;
    }
    private void OnDisable()
    {
        TDGameManager.OnEndGameResetPools -= ResetPools;
    }
    public static AreaStateFactory GetInstance()
    {
        return instance;
    }
    private void Init()
    {
        sortedAreas = new Dictionary<AreaFunctionality.AreaType, Pool>();
        foreach (AreaTypeToPool areaTypeToPool in areasToPool)
        {
            sortedAreas[areaTypeToPool.type] = areaTypeToPool.pool;
        }
    }

    public GameObject GetAreaGameObject(AreaFunctionality.AreaType areaType,Vector3 position,Quaternion rotation,Transform parent)
    {
        return sortedAreas[areaType].GetObject(position, rotation, parent);
    }

    void ResetPools()
    {
        foreach (AreaTypeToPool areaTypeToPool in areasToPool)
        {
            areaTypeToPool.pool.ResetObjectsList();
        }
    }
}
