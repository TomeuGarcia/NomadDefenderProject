using System;
using Scripts.ObjectPooling;
using UnityEngine;

public class CardAbilityTooltipFactory : MonoBehaviour, ICardAbilityTooltipFactory
{
    [SerializeField] private Transform _tooltipsSpawnParent;
    [SerializeField] private ObjectPoolData<CardAbilityTooltip> _abilitiesTooltipsPoolData;
    [SerializeField] private ObjectPoolData<CardAbilityTooltip> _keywordsTooltipsPoolData;
    
    private ObjectPool _abilitiesTooltipsPool;
    private ObjectPool _keywordsTooltipsPool;

    private void Awake()
    {
        _abilitiesTooltipsPool = _abilitiesTooltipsPoolData.ToObjectPool(_tooltipsSpawnParent);
        _keywordsTooltipsPool = _keywordsTooltipsPoolData.ToObjectPool(_tooltipsSpawnParent);
    }


    public CardAbilityTooltip CreateAbilityTooltip()
    {
        CardAbilityTooltip cardAbilityTooltip = _abilitiesTooltipsPool.Spawn<CardAbilityTooltip>(Vector3.zero, Quaternion.identity);
        cardAbilityTooltip.SetOriginalParent(_tooltipsSpawnParent);
        return cardAbilityTooltip;
    }
    
    public CardAbilityTooltip CreateKeywordTooltip()
    {
        CardAbilityTooltip cardAbilityTooltip = _keywordsTooltipsPool.Spawn<CardAbilityTooltip>(Vector3.zero, Quaternion.identity);
        cardAbilityTooltip.SetOriginalParent(_tooltipsSpawnParent);
        return cardAbilityTooltip;
    }
}