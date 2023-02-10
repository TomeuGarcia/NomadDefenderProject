using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartsLibrary", menuName = "TurretParts/TurretPartsLibrary")]
public class PartsLibrary : ScriptableObject
{
    [System.Serializable]
    public struct AttackAndPowerfulness
    {
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int powerfulness;
        public TurretPartAttack attackPart;
    }

    [System.Serializable]
    public struct BodyAndPowerfulness
    {
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int powerfulness;
        public TurretPartBody bodyPart;
    }

    [System.Serializable]
    public struct BasePassiveBaseAndPowerfulness
    {
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int powerfulness;
        public TurretPartBase basePart;
        public TurretPassiveBase basePassivePart;
    }

    public AttackAndPowerfulness[] attackPartsByPowerfulness;
    public BodyAndPowerfulness[] bodyPartsByPowerfulness;
    public BasePassiveBaseAndPowerfulness[] baseAndPassiveBasePartsByPowerfulness;


    [System.Serializable]
    public struct PartGenerationClassification
    {
        public NodeEnums.ProgressionState progressionState;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int minPowerfulness;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int maxPowerfulness;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int minPerfectPowerfulness;
        [Range(GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS)] public int maxPerfectPowerfulness;
    }
    [Header("GENERATION CLASSIFICATION")]
    [SerializeField] public PartGenerationClassification[] partsGenerationClassifications;


    public AttackAndPowerfulness GetRandomTurretPartAttackInCollection(AttackAndPowerfulness[] attackAndPowerfulnesses)
    {
        return attackAndPowerfulnesses[Random.Range(0, attackAndPowerfulnesses.Length)];
    }
    public BodyAndPowerfulness GetRandomTurretPartBodyInCollection(BodyAndPowerfulness[] bodyAndPowerfulnesses)
    {
        return bodyAndPowerfulnesses[Random.Range(0, bodyAndPowerfulnesses.Length)];
    }
    public BasePassiveBaseAndPowerfulness GetRandomTurretPartBaseAndPassiveInCollection(BasePassiveBaseAndPowerfulness[] basePassiveBaseAndPowerfulness)
    {
        return basePassiveBaseAndPowerfulness[Random.Range(0, basePassiveBaseAndPowerfulness.Length)];
    }



    private bool IsPowerfulnessInRange(AttackAndPowerfulness partAndPowerfulness, int minPowerfulness, int maxPowerfulness)
    {
        return partAndPowerfulness.powerfulness >= minPowerfulness && partAndPowerfulness.powerfulness <= maxPowerfulness;
    }
    private bool IsPowerfulnessInRange(BodyAndPowerfulness partAndPowerfulness, int minPowerfulness, int maxPowerfulness)
    {
        return partAndPowerfulness.powerfulness >= minPowerfulness && partAndPowerfulness.powerfulness <= maxPowerfulness;
    }
    private bool IsPowerfulnessInRange(BasePassiveBaseAndPowerfulness partAndPowerfulness, int minPowerfulness, int maxPowerfulness)
    {
        return partAndPowerfulness.powerfulness >= minPowerfulness && partAndPowerfulness.powerfulness <= maxPowerfulness;
    }
    private PartGenerationClassification GetClassificationByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        PartGenerationClassification classificationByProgress;
        int i = 0;

        do
        {
            classificationByProgress = partsGenerationClassifications[i++];
        }
        while (classificationByProgress.progressionState != progressionState && i < partsGenerationClassifications.Length);


        return classificationByProgress;
    }



    // ATTACK PARTS
    public TurretPartAttack[] GetRandomTurretPartAttacks(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        if (totalAmount > attackPartsByPowerfulness.Length) totalAmount = attackPartsByPowerfulness.Length;


        PartGenerationClassification classificationByProgress = GetClassificationByProgressionState(progressionState);


        HashSet<TurretPartAttack> holderPartsSet = new HashSet<TurretPartAttack>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(GetRandomAttackPart(classificationByProgress.minPerfectPowerfulness, classificationByProgress.maxPerfectPowerfulness));
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(GetRandomAttackPart(classificationByProgress.minPowerfulness, classificationByProgress.maxPowerfulness));
        }


        return holderPartsSet.ToArray();
    }

    private TurretPartAttack GetRandomAttackPart(int minPowerfulness, int maxPowerfulness)
    {
        AttackAndPowerfulness[] partsInRange = GetAttackPartsInRange(minPowerfulness, maxPowerfulness);

        AttackAndPowerfulness attackAndPowerfulness;
        int tries = 0;
        int MAX_TRIES = 30;

        do
        {
            attackAndPowerfulness = GetRandomTurretPartAttackInCollection(partsInRange);
        }
        while (!IsPowerfulnessInRange(attackAndPowerfulness, minPowerfulness, maxPowerfulness) && tries++ < MAX_TRIES);


        if (tries >= MAX_TRIES) Debug.Log("POSSIBLE ERROR: Couldn't find Attack Part in range [" + minPowerfulness + ", " + maxPowerfulness + "]");


        return attackAndPowerfulness.attackPart;
    }

    private AttackAndPowerfulness[] GetAttackPartsInRange(int minPowerfulness, int maxPowerfulness)
    {
        List<AttackAndPowerfulness> partsAndPowerfulness = new List<AttackAndPowerfulness>();

        for (int i = 0; i < attackPartsByPowerfulness.Length; ++i)
        {
            if (IsPowerfulnessInRange(attackPartsByPowerfulness[i], minPowerfulness, maxPowerfulness))
            {
                partsAndPowerfulness.Add(attackPartsByPowerfulness[i]);
            }
        }

        return partsAndPowerfulness.ToArray();
    }



    // BODY PARTS
    public TurretPartBody[] GetRandomTurretPartBodies(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        if (totalAmount > bodyPartsByPowerfulness.Length) totalAmount = bodyPartsByPowerfulness.Length;


        PartGenerationClassification classificationByProgress = GetClassificationByProgressionState(progressionState);


        HashSet<TurretPartBody> holderPartsSet = new HashSet<TurretPartBody>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(GetRandomBodyPart(classificationByProgress.minPerfectPowerfulness, classificationByProgress.maxPerfectPowerfulness));
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(GetRandomBodyPart(classificationByProgress.minPowerfulness, classificationByProgress.maxPowerfulness));
        }


        return holderPartsSet.ToArray();
    }

    private TurretPartBody GetRandomBodyPart(int minPowerfulness, int maxPowerfulness)
    {
        BodyAndPowerfulness[] partsInRange = GetBodyPartsInRange(minPowerfulness, maxPowerfulness);

        BodyAndPowerfulness bodyAndPowerfulness;
        int tries = 0;
        int MAX_TRIES = 30;

        do
        {
            bodyAndPowerfulness = GetRandomTurretPartBodyInCollection(partsInRange);
        }
        while (!IsPowerfulnessInRange(bodyAndPowerfulness, minPowerfulness, maxPowerfulness) && tries++ < MAX_TRIES);


        if (tries >= MAX_TRIES) Debug.Log("POSSIBLE ERROR: Couldn't find Body Part in range [" + minPowerfulness + ", " + maxPowerfulness + "]");


        return bodyAndPowerfulness.bodyPart;
    }

    private BodyAndPowerfulness[] GetBodyPartsInRange(int minPowerfulness, int maxPowerfulness)
    {
        List<BodyAndPowerfulness> partsAndPowerfulness = new List<BodyAndPowerfulness>();

        for (int i = 0; i < bodyPartsByPowerfulness.Length; ++i)
        {
            if (IsPowerfulnessInRange(bodyPartsByPowerfulness[i], minPowerfulness, maxPowerfulness))
            {
                partsAndPowerfulness.Add(bodyPartsByPowerfulness[i]);
            }
        }

        return partsAndPowerfulness.ToArray();
    }



    // BASE PARTS
    public (TurretPartBase, TurretPassiveBase)[] GetRandomTurretPartBaseAndPassive(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        if (totalAmount > baseAndPassiveBasePartsByPowerfulness.Length) totalAmount = baseAndPassiveBasePartsByPowerfulness.Length;


        PartGenerationClassification classificationByProgress = GetClassificationByProgressionState(progressionState);


        HashSet<(TurretPartBase, TurretPassiveBase)> holderPartsSet = new HashSet<(TurretPartBase, TurretPassiveBase)>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(GetRandomBaseAndPassivePart(classificationByProgress.minPerfectPowerfulness, classificationByProgress.maxPerfectPowerfulness));
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(GetRandomBaseAndPassivePart(classificationByProgress.minPowerfulness, classificationByProgress.maxPowerfulness));
        }


        return holderPartsSet.ToArray();
    }

    private (TurretPartBase, TurretPassiveBase) GetRandomBaseAndPassivePart(int minPowerfulness, int maxPowerfulness)
    {
        BasePassiveBaseAndPowerfulness[] partsInRange = GetBaseAndPassivePartsInRange(minPowerfulness, maxPowerfulness);

        BasePassiveBaseAndPowerfulness baseAndPasssiveAndPowerfulness;
        int tries = 0;
        int MAX_TRIES = 30;

        do
        {
            baseAndPasssiveAndPowerfulness = GetRandomTurretPartBaseAndPassiveInCollection(partsInRange);
        }
        while (!IsPowerfulnessInRange(baseAndPasssiveAndPowerfulness, minPowerfulness, maxPowerfulness) && tries++ < MAX_TRIES);


        if (tries >= MAX_TRIES) Debug.Log("POSSIBLE ERROR: Couldn't find BaseAndPassiveBase Part in range [" + minPowerfulness + ", " + maxPowerfulness + "]");


        return (baseAndPasssiveAndPowerfulness.basePart, baseAndPasssiveAndPowerfulness.basePassivePart);
    }

    private BasePassiveBaseAndPowerfulness[] GetBaseAndPassivePartsInRange(int minPowerfulness, int maxPowerfulness)
    {
        List<BasePassiveBaseAndPowerfulness> partsAndPowerfulness = new List<BasePassiveBaseAndPowerfulness>();

        for (int i = 0; i < baseAndPassiveBasePartsByPowerfulness.Length; ++i)
        {
            if (IsPowerfulnessInRange(baseAndPassiveBasePartsByPowerfulness[i], minPowerfulness, maxPowerfulness))
            {
                partsAndPowerfulness.Add(baseAndPassiveBasePartsByPowerfulness[i]);
            }
        }

        return partsAndPowerfulness.ToArray();
    }

}
