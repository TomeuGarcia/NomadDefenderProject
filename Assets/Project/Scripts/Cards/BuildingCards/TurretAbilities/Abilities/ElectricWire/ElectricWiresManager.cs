

using System;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class ElectricWiresManager : MonoBehaviour
{
    [SerializeField] private ObjectPoolData<ElectricWireSegment> _wireSegmentsPoolData;
    private ObjectPool _wireSegmentsPool;

    private List<ElectricWireSegment> _activeWireSegments;

    public static ElectricWiresManager Instance { get; private set; } 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        TDGameManager.OnSceneFinish += RemoveAllSegments;
    }
    private void OnDisable()
    {
        TDGameManager.OnSceneFinish -= RemoveAllSegments;
    }


    private void Init()
    {
        _wireSegmentsPool = _wireSegmentsPoolData.ToObjectPool(transform);
        _activeWireSegments = new List<ElectricWireSegment>(5);
    }



    public void CreateSegment(ElectricWireSegment.IEnemyDamageData enemyDamageData, 
        ElectricWireSegment.IAttachable wireOwner, ElectricWireSegment.IAttachable otherAttachable)
    {
        foreach (ElectricWireSegment activeWireSegment in _activeWireSegments)
        {
            if (activeWireSegment.HasOwner(otherAttachable) && activeWireSegment.HasOther(wireOwner))
            {
                activeWireSegment.SetMultipleOwners(wireOwner, otherAttachable);
                return;
            }
        }
        
        
        ElectricWireSegment segment =
            _wireSegmentsPool.Spawn<ElectricWireSegment>(wireOwner.GetAttachPosition(), Quaternion.identity);
        
        segment.SetEnemyDamageData(enemyDamageData);
        segment.SetSingleOwner(wireOwner, otherAttachable);
        segment.SetPlacement(wireOwner.GetAttachPosition(), otherAttachable.GetAttachPosition());
        _activeWireSegments.Add(segment);
    }

    public void RemoveSegment(ElectricWireSegment.IAttachable wireOwner, ElectricWireSegment.IAttachable otherAttachable)
    {
        for (int i = 0; i < _activeWireSegments.Count; ++i)
        {
            ElectricWireSegment segment = _activeWireSegments[i];
            if (segment.HasOwner(wireOwner) && segment.HasOther(otherAttachable))
            {
                segment.DoGetRemoved();
                _activeWireSegments.RemoveAt(i);
                return;
            }
        }
    }
    
    public void RemoveAllSegments(ElectricWireSegment.IAttachable wireOwner)
    {
        for (int i = _activeWireSegments.Count - 1; i >= 0; --i)
        {
            ElectricWireSegment segment = _activeWireSegments[i];
            if (segment.HasOwner(wireOwner))
            {
                segment.DoGetRemoved();
                _activeWireSegments.RemoveAt(i);
            }
        }
    }

    private void RemoveAllSegments()
    {
        foreach (ElectricWireSegment segment in _activeWireSegments)
        {
            segment.DoGetRemoved();
        }
        _activeWireSegments.Clear();
    }

}