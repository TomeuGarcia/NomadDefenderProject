using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupedCardParts : ScriptableObject
{
    [HideInInspector] public const int MAX_CARD_POWERFULNESS = 100;
    [HideInInspector] public const int MIN_CARD_POWERFULNESS = 1;
    [SerializeField, Range(MIN_CARD_POWERFULNESS, MAX_CARD_POWERFULNESS)] private int cardPowerfulness = 50;

    [Header("")]

    [SerializeField, Min(0)] public int cardCost;


    public int GetCardPowerfulness()
    {
        return cardPowerfulness;
    }
    public bool IsPowerfulnessInRange(int minPowerfulnessInclusive, int maxPowerfulnessInclusive)
    {
        return cardPowerfulness >= minPowerfulnessInclusive && cardPowerfulness <= maxPowerfulnessInclusive;
    }

}
