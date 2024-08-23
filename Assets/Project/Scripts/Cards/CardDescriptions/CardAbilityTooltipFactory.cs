using System;
using Scripts.ObjectPooling;
using UnityEngine;

public class CardAbilityTooltipFactory : MonoBehaviour, ICardAbilityTooltipFactory
{
    [SerializeField] private Transform _tooltipsSpawnParent;
    [SerializeField] private ObjectPoolData<CardAbilityTooltip> _tooltipsPoolData;
    private ObjectPool _tooltipsPool;

    private void Awake()
    {
        _tooltipsPool = _tooltipsPoolData.ToObjectPool(_tooltipsSpawnParent);
    }


    public CardAbilityTooltip CreateCardAbilityTooltip()
    {
        CardAbilityTooltip cardAbilityTooltip = _tooltipsPool.Spawn<CardAbilityTooltip>(Vector3.zero, Quaternion.identity);
        return cardAbilityTooltip;
    }
}