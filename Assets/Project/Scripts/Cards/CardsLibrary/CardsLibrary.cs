using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsLibrary", menuName = "Cards/CardsLibrary")]
public class CardsLibrary : ScriptableObject
{
    //DON'T REMOVE SerializeFields, otherwise ScriptableObject doesn't save the arrays when SetContent()
    [SerializeField] private CardsLibraryContent content;

    private TurretCardParts[] TotalTurretCardParts => content.totalTurretCardParts;
    private SupportCardParts[] TotalSupportCardParts => content.totalSupportCardParts;


    private int minPowerfulnessTurretRegistered = 0;
    private int maxPowerfulnessTurretRegistered = 0;

    private int minPowerfulnessSupportRegistered = 0;
    private int maxPowerfulnessSupportRegistered = 0;


    [System.Serializable]
    public struct CardGenerationClassification
    {
        public NodeEnums.ProgressionState progressionState;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int minPowerfulness;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int maxPowerfulness;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int minPerfectPowerfulness;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int maxPerfectPowerfulness;       
    }
    [Header("GENERATION CLASSIFICATION")]
    [SerializeField] public CardGenerationClassification[] cardGenerationClassifications;


    public void SetContent(CardsLibraryContent newContent)
    {
        content = newContent;
    }

    public void InitSetup()
    {
        minPowerfulnessTurretRegistered = GroupedCardParts.MAX_CARD_POWERFULNESS;
        maxPowerfulnessTurretRegistered = GroupedCardParts.MIN_CARD_POWERFULNESS;

        for (int i = 0; i < TotalTurretCardParts.Length; ++i)
        {
            int cardPowerfulness = TotalTurretCardParts[i].GetCardPowerfulness();

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

        for (int i = 0; i < TotalSupportCardParts.Length; ++i)
        {
            int cardPowerfulness = TotalSupportCardParts[i].GetCardPowerfulness();

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
    private TurretCardParts GetRandomTurretCardPartsInCollection(TurretCardParts[] turretCardParts)
    {
        return turretCardParts[Random.Range(0, turretCardParts.Length)];
    }
    public TurretCardParts GetRandomTurretCardParts()
    {
        return GetRandomTurretCardPartsInCollection(TotalTurretCardParts);
    }

    private SupportCardParts GetRandomSupportCardPartsInCollection(SupportCardParts[] supportCardParts)
    {
        return supportCardParts[Random.Range(0, supportCardParts.Length)];
    }
    public SupportCardParts GetRandomSupportCardParts()
    {
        return GetRandomSupportCardPartsInCollection(TotalSupportCardParts);
    }


    public TurretCardParts[] GetRandomTurretCardPartsSet(int amount)
    {
        if (amount > TotalTurretCardParts.Length) amount = TotalTurretCardParts.Length;

        HashSet<TurretCardParts> partsSet = new HashSet<TurretCardParts>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomTurretCardParts());
        }

        return partsSet.ToArray();
    }

    public SupportCardParts[] GetRandomSupportCardPartsSet(int amount)
    {
        if (amount > TotalSupportCardParts.Length) amount = TotalSupportCardParts.Length;

        HashSet<SupportCardParts> partsSet = new HashSet<SupportCardParts>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomSupportCardParts());
        }

        return partsSet.ToArray();
    }



    // ---- RANDOM WITHIN POWERFULNESS RANGE ----
    private TurretCardParts[] GetTurretCardPartsInRange(int minPowerfulness, int maxPowerfulness)
    {
        List<TurretCardParts> turretCardParts = new List<TurretCardParts>();

        for (int i = 0; i < TotalTurretCardParts.Length; ++i)
        {
            if (TotalTurretCardParts[i].IsPowerfulnessInRange(minPowerfulness, maxPowerfulness))
            {
                turretCardParts.Add(TotalTurretCardParts[i]);
            }
        }

        return turretCardParts.ToArray();
    }
    private SupportCardParts[] GetSupportCardPartsInRange(int minPowerfulness, int maxPowerfulness)
    {
        List<SupportCardParts> turretCardParts = new List<SupportCardParts>();

        for (int i = 0; i < TotalSupportCardParts.Length; ++i)
        {
            if (TotalSupportCardParts[i].IsPowerfulnessInRange(minPowerfulness, maxPowerfulness))
            {
                turretCardParts.Add(TotalSupportCardParts[i]);
            }
        }

        return turretCardParts.ToArray();
    }





    public TurretCardParts GetRandomTurretCardParts(int minPowerfulness, int maxPowerfulness)
    {
        minPowerfulness = Mathf.Max(minPowerfulnessTurretRegistered, minPowerfulness);
        maxPowerfulness = Mathf.Min(maxPowerfulnessTurretRegistered, maxPowerfulness);

        TurretCardParts[] turretCardPartsInRange = GetTurretCardPartsInRange(minPowerfulness, maxPowerfulness);

        TurretCardParts turretCardParts = null;
        int tries = 0;
        int MAX_TRIES = 30;

        do
        {
            turretCardParts = GetRandomTurretCardPartsInCollection(turretCardPartsInRange);
        }
        while(!turretCardParts.IsPowerfulnessInRange(minPowerfulness, maxPowerfulness) && tries++ < MAX_TRIES);


        if (tries >= MAX_TRIES) Debug.Log("POSSIBLE ERROR: Couldn't find Turret Card in range [" + minPowerfulness + ", " + maxPowerfulness + "]");


        return turretCardParts;
    }


    public SupportCardParts GetRandomSupportCardParts(int minPowerfulness, int maxPowerfulness)
    {
        minPowerfulness = Mathf.Max(minPowerfulnessSupportRegistered, minPowerfulness);
        maxPowerfulness = Mathf.Min(maxPowerfulnessSupportRegistered, maxPowerfulness);

        SupportCardParts[] supportCardPartsInRange = GetSupportCardPartsInRange(minPowerfulness, maxPowerfulness);        

        SupportCardParts supportCardParts = null;
        int tries = 0;
        int MAX_TRIES = 30;

        do
        {
            supportCardParts = GetRandomSupportCardPartsInCollection(supportCardPartsInRange);
        }
        while (!supportCardParts.IsPowerfulnessInRange(minPowerfulness, maxPowerfulness) && tries++ < MAX_TRIES);

        if (tries >= MAX_TRIES) Debug.Log("POSSIBLE ERROR: Couldn't find Support Card in range [" + minPowerfulness + ", " + maxPowerfulness + "]");

        return supportCardParts;
    }


    public TurretCardParts[] GetRandomTurretCardPartsSet(int amount, int minPowerfulness, int maxPowerfulness)
    {
        if (amount > TotalTurretCardParts.Length) amount = TotalTurretCardParts.Length;

        HashSet<TurretCardParts> holderPartsSet = new HashSet<TurretCardParts>();
        while (holderPartsSet.Count < amount)
        {
            holderPartsSet.Add(GetRandomTurretCardParts(minPowerfulness, maxPowerfulness));
        }


        return holderPartsSet.ToArray();
    }

    public SupportCardParts[] GetRandomSupportCardPartsSet(int amount, int minPowerfulness, int maxPowerfulness)
    {
        if (amount > TotalSupportCardParts.Length) amount = TotalSupportCardParts.Length;

        HashSet<SupportCardParts> partsSet = new HashSet<SupportCardParts>();
        while (partsSet.Count < amount)
        {
            partsSet.Add(GetRandomSupportCardParts(minPowerfulness, maxPowerfulness));
        }

        return partsSet.ToArray();
    }




    private CardGenerationClassification GetClassificationByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        CardGenerationClassification classificationByProgress;
        int i = 0;

        do
        {
            classificationByProgress = cardGenerationClassifications[i++];
        }
        while (classificationByProgress.progressionState != progressionState && i < cardGenerationClassifications.Length);


        return classificationByProgress;
    }

    public TurretCardParts[] GetRandomByProgressionTurretCardPartsSet(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        if (totalAmount > TotalTurretCardParts.Length) totalAmount = TotalTurretCardParts.Length;


        CardGenerationClassification classificationByProgress = GetClassificationByProgressionState(progressionState);


        HashSet<TurretCardParts> holderPartsSet = new HashSet<TurretCardParts>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(GetRandomTurretCardParts(classificationByProgress.minPerfectPowerfulness, classificationByProgress.maxPerfectPowerfulness));
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(GetRandomTurretCardParts(classificationByProgress.minPowerfulness, classificationByProgress.maxPowerfulness));
        }


        return holderPartsSet.ToArray();
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