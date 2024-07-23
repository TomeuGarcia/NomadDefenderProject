using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartsLibrary", menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "TurretPartsLibrary")]
public class PartsLibrary : ScriptableObject
{
    [System.Serializable]
    public struct AttacksByProgressionState
    {
        public NodeEnums.ProgressionState progressionState;
        public TurretPartAttack[] parts;
        public TurretPartAttack[] perfectParts;

        public TurretPartAttack GetRandomPart()
        {
            return parts[Random.Range(0, parts.Length)];
        }
        public TurretPartAttack GetRandomPerfectPart()
        {
            return perfectParts[Random.Range(0, perfectParts.Length)];
        }
    }


    [System.Serializable]
    public struct BodiesByProgressionState
    {
        public NodeEnums.ProgressionState progressionState;
        public TurretPartBody[] parts;
        public TurretPartBody[] perfectParts;

        public TurretPartBody GetRandomPart()
        {
            return parts[Random.Range(0, parts.Length)];
        }
        public TurretPartBody GetRandomPerfectPart()
        {
            return perfectParts[Random.Range(0, perfectParts.Length)];
        }
    }

    [System.Serializable]
    public struct BaseAndPassive
    {
        public TurretPartBase turretPartBase;
        public TurretPassiveBase turretPassiveBase;


        public static bool operator== (BaseAndPassive obj1, BaseAndPassive obj2)
        {
            if (obj1 == null || obj2 == null) return false;
            return obj1.turretPassiveBase == obj2.turretPassiveBase;
        }
        public static bool operator !=(BaseAndPassive obj1, BaseAndPassive obj2)
        {
            return !(obj1 == obj2);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return System.HashCode.Combine(turretPartBase, turretPassiveBase);
        }
    }

    [System.Serializable]
    public struct BasesAndPassivesByProgressionState
    {
        public NodeEnums.ProgressionState progressionState;
        public BaseAndPassive[] parts;
        public BaseAndPassive[] perfectParts;

        public BaseAndPassive GetRandomPart()
        {
            return parts[Random.Range(0, parts.Length)];
        }
        public BaseAndPassive GetRandomPerfectPart()
        {
            return perfectParts[Random.Range(0, perfectParts.Length)];
        }
    }

    [System.Serializable]
    public struct BonusStatsByProgressionState
    {
        public NodeEnums.ProgressionState progressionState;
        public TurretStatsUpgradeModel[] parts;
        public TurretStatsUpgradeModel[] perfectParts;

        public TurretStatsUpgradeModel GetRandomPart()
        {
            return parts[Random.Range(0, parts.Length)];
        }
        public TurretStatsUpgradeModel GetRandomPerfectPart()
        {
            return perfectParts[Random.Range(0, perfectParts.Length)];
        }
    }



    private AttacksByProgressionState[] attacksByProgressionStates;
    private BodiesByProgressionState[] bodiesByProgressionStates;
    private BasesAndPassivesByProgressionState[] basesAndPassivesByProgressionStates;
    private BonusStatsByProgressionState[] bonusStatsByProgressionStates;


    public void SetContent(AttackPartsLibraryContent newAttacksContent, BodyPartsLibraryContent newBodiesContent, 
        BasePartsLibraryContent newBasesContent, BonusStatsPartsLibraryContent newBonusStatsContent)
    {
        attacksByProgressionStates = newAttacksContent.GetArrayByProgression();
        bodiesByProgressionStates = newBodiesContent.GetArrayByProgression();
        basesAndPassivesByProgressionStates = newBasesContent.GetArrayByProgression();
        bonusStatsByProgressionStates = newBonusStatsContent.GetArrayByProgression();
    }


    // ATTACK PARTS
    private AttacksByProgressionState GetAttacksByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < attacksByProgressionStates.Length; ++i)
        {
            if (attacksByProgressionStates[i].progressionState == progressionState)
                return attacksByProgressionStates[i];
        }

        return attacksByProgressionStates[0];
    }

    public TurretPartAttack[] GetRandomTurretPartAttacks(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, attacksByProgressionStates.Length);
        AttacksByProgressionState attacksByProgressionState = GetAttacksByProgressionState(progressionState);
        HashSet<TurretPartAttack> holderPartsSet = new HashSet<TurretPartAttack>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(attacksByProgressionState.GetRandomPerfectPart());
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(attacksByProgressionState.GetRandomPart());
        }


        return holderPartsSet.ToArray();
    }


    // BODY PARTS
    private BodiesByProgressionState GetBodiesByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < bodiesByProgressionStates.Length; ++i)
        {
            if (bodiesByProgressionStates[i].progressionState == progressionState)
                return bodiesByProgressionStates[i];
        }

        return bodiesByProgressionStates[0];
    }

    public TurretPartBody[] GetRandomTurretPartBodies(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, bodiesByProgressionStates.Length);
        BodiesByProgressionState bodiesByProgressionState = GetBodiesByProgressionState(progressionState);
        HashSet<TurretPartBody> holderPartsSet = new HashSet<TurretPartBody>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(bodiesByProgressionState.GetRandomPerfectPart());
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(bodiesByProgressionState.GetRandomPart());
        }


        return holderPartsSet.ToArray();
    }


    // BASE AND PASSIVE PARTS
    private BasesAndPassivesByProgressionState GetBasesAndPassivesByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < basesAndPassivesByProgressionStates.Length; ++i)
        {
            if (basesAndPassivesByProgressionStates[i].progressionState == progressionState)
                return basesAndPassivesByProgressionStates[i];
        }

        return basesAndPassivesByProgressionStates[0];
    }

    public BaseAndPassive[] GetRandomTurretPartBaseAndPassive(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, basesAndPassivesByProgressionStates.Length);
        BasesAndPassivesByProgressionState basesAndPassivesByProgressionState = GetBasesAndPassivesByProgressionState(progressionState);
        HashSet<BaseAndPassive> holderPartsSet = new HashSet<BaseAndPassive>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(basesAndPassivesByProgressionState.GetRandomPerfectPart());

                /*
                BaseAndPassive baseAndPassive = basesAndPassivesByProgressionState.GetRandomPerfectPart();
                if (!holderPartsSet.Contains(baseAndPassive))
                {
                    holderPartsSet.Add(baseAndPassive);
                } 
                */
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            BaseAndPassive randomPart = basesAndPassivesByProgressionState.GetRandomPart();
            bool canAdd = true;

            foreach (BaseAndPassive part in holderPartsSet) 
            {
                if (part.turretPassiveBase == randomPart.turretPassiveBase)
                    canAdd = false;
            }

            if (canAdd) holderPartsSet.Add(randomPart);
        }


        return holderPartsSet.ToArray();
    }



    // BONUS STATS PARTS
    private BonusStatsByProgressionState GetBonusStatsByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < bonusStatsByProgressionStates.Length; ++i)
        {
            if (bonusStatsByProgressionStates[i].progressionState == progressionState)
                return bonusStatsByProgressionStates[i];
        }

        return bonusStatsByProgressionStates[0];
    }

    public TurretStatsUpgradeModel[] GetRandomTurretStatsUpgradeModel(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, bonusStatsByProgressionStates.Length);
        BonusStatsByProgressionState bonusStatsByProgressionState = GetBonusStatsByProgressionState(progressionState);
        HashSet<TurretStatsUpgradeModel> holderPartsSet = new HashSet<TurretStatsUpgradeModel>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(bonusStatsByProgressionState.GetRandomPerfectPart());
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(bonusStatsByProgressionState.GetRandomPart());
        }


        return holderPartsSet.ToArray();
    }
}
