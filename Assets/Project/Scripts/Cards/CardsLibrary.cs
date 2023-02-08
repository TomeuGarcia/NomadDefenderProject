using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsLibrary", menuName = "Cards/CardsLibrary")]
public class CardsLibrary : ScriptableObject
{
    public TurretCardParts[] totalTurretCardParts;
    public SupportCardParts[] totalSupportCardParts;


    private int minPowerfulnessTurretRegistered = 0;
    private int maxPowerfulnessTurretRegistered = 0;

    private int minPowerfulnessSupportRegistered = 0;
    private int maxPowerfulnessSupportRegistered = 0;
  


    public void InitSetup()
    {
        minPowerfulnessTurretRegistered = GroupedCardParts.MAX_CARD_POWERFULNESS;
        maxPowerfulnessTurretRegistered = GroupedCardParts.MIN_CARD_POWERFULNESS;

        for (int i = 0; i < totalTurretCardParts.Length; ++i)
        {
            int cardPowerfulness = totalTurretCardParts[i].GetCardPowerfulness();

            if (cardPowerfulness < minPowerfulnessTurretRegistered)
            {
                minPowerfulnessTurretRegistered = cardPowerfulness;
            }

            if (cardPowerfulness > maxPowerfulnessTurretRegistered)
            {
                maxPowerfulnessTurretRegistered = cardPowerfulness;
            }
        }


        minPowerfulnessSupportRegistered = GroupedCardParts.MAX_CARD_POWERFULNESS;
        maxPowerfulnessSupportRegistered = GroupedCardParts.MIN_CARD_POWERFULNESS;

        for (int i = 0; i < totalSupportCardParts.Length; ++i)
        {
            int cardPowerfulness = totalSupportCardParts[i].GetCardPowerfulness();

            if (cardPowerfulness < minPowerfulnessSupportRegistered)
            {
                minPowerfulnessSupportRegistered = cardPowerfulness;
            }

            if (cardPowerfulness > maxPowerfulnessSupportRegistered)
            {
                maxPowerfulnessSupportRegistered = cardPowerfulness;
            }
        }
    }



    // ---- COMPLETELY RANDOM ----
    public TurretCardParts GetRandomTurretCardParts()
    {
        return totalTurretCardParts[Random.Range(0, totalTurretCardParts.Length)];
    }

    public SupportCardParts GetRandomSupportCardParts()
    {
        return totalSupportCardParts[Random.Range(0, totalSupportCardParts.Length)];
    }


    public TurretCardParts[] GetRandomTurretCardPartsSet(int amount)
    {
        if (amount > totalTurretCardParts.Length) amount = totalTurretCardParts.Length;

        HashSet<TurretCardParts> partsSet = new HashSet<TurretCardParts>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomTurretCardParts());
        }

        return partsSet.ToArray();
    }

    public SupportCardParts[] GetRandomSupportCardPartsSet(int amount)
    {
        if (amount > totalSupportCardParts.Length) amount = totalSupportCardParts.Length;

        HashSet<SupportCardParts> partsSet = new HashSet<SupportCardParts>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomSupportCardParts());
        }

        return partsSet.ToArray();
    }



    // ---- RANDOM WITHIN POWERFULNESS RANGE ----
    public TurretCardParts GetRandomTurretCardParts(int minPowerfulness, int maxPowerfulness)
    {
        minPowerfulness = Mathf.Max(minPowerfulnessTurretRegistered, minPowerfulness);
        maxPowerfulness = Mathf.Min(maxPowerfulnessTurretRegistered, maxPowerfulness);

        TurretCardParts turretCardParts = null;

        do
        {
            turretCardParts = GetRandomTurretCardParts();
        }
        while(turretCardParts.IsPowerfulnessInRange(minPowerfulness, maxPowerfulness));

        return turretCardParts;
    }

    public SupportCardParts GetRandomSupportCardParts(int minPowerfulness, int maxPowerfulness)
    {
        minPowerfulness = Mathf.Max(minPowerfulnessSupportRegistered, minPowerfulness);
        maxPowerfulness = Mathf.Min(maxPowerfulnessSupportRegistered, maxPowerfulness);

        SupportCardParts supportCardParts = null;

        do
        {
            supportCardParts = GetRandomSupportCardParts();
        }
        while (supportCardParts.IsPowerfulnessInRange(minPowerfulness, maxPowerfulness));

        return supportCardParts;
    }


    public TurretCardParts[] GetRandomTurretCardPartsSet(int amount, int minPowerfulness, int maxPowerfulness)
    {
        if (amount > totalTurretCardParts.Length) amount = totalTurretCardParts.Length;

        HashSet<TurretCardParts> holderPartsSet = new HashSet<TurretCardParts>();
        while (holderPartsSet.Count < amount)
        {
            holderPartsSet.Add(GetRandomTurretCardParts(minPowerfulness, maxPowerfulness));
        }


        return holderPartsSet.ToArray();
    }

    public SupportCardParts[] GetRandomSupportCardPartsSet(int amount, int minPowerfulness, int maxPowerfulness)
    {
        if (amount > totalSupportCardParts.Length) amount = totalSupportCardParts.Length;

        HashSet<SupportCardParts> partsSet = new HashSet<SupportCardParts>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomSupportCardParts(minPowerfulness, maxPowerfulness));
        }

        return partsSet.ToArray();
    }



    // ---- USE TO CREATE NEW CARDS (INPUT RANDOM GENERATED CARDPARTS FROM ABOVE) ----
    public TurretCardParts GetConsumableTurretCardParts(TurretCardParts holderParts)
    {
        TurretCardParts parts = ScriptableObject.CreateInstance("TurretCardParts") as TurretCardParts;
        parts.Init(holderParts);

        return parts;
    }

    public SupportCardParts GetConsumableSupportCardParts(SupportCardParts holderParts)
    {
        SupportCardParts parts = ScriptableObject.CreateInstance("SupportCardParts") as SupportCardParts;
        parts.Init(holderParts);

        return parts;
    }


    public TurretCardParts[] GetConsumableTurretCardPartsSet(TurretCardParts[] holderPartsSetArray)
    {
        TurretCardParts[] partsSet = new TurretCardParts[holderPartsSetArray.Length];
        for (int i = 0; i < holderPartsSetArray.Length; ++i)
        {
            partsSet[i] = GetConsumableTurretCardParts(holderPartsSetArray[i]);
        }

        return partsSet;
    }

    public SupportCardParts[] GetConsumableSupportCardPartsSet(SupportCardParts[] holderPartsSetArray)
    {
        SupportCardParts[] partsSet = new SupportCardParts[holderPartsSetArray.Length];
        for (int i = 0; i < holderPartsSetArray.Length; ++i)
        {
            partsSet[i] = GetConsumableSupportCardParts(holderPartsSetArray[i]);
        }

        return partsSet;
    }

}