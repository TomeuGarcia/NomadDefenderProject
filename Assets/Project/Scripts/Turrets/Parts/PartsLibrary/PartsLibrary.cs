using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartsLibrary", menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "TurretPartsLibrary")]
public class PartsLibrary : ScriptableObject
{
    [System.Serializable]
    public struct PartsByProgressionState<T>
    {
        public NodeEnums.ProgressionState progressionState;
        public T[] parts;
        public T[] perfectParts;

        public T GetRandomPart()
        {
            return parts[Random.Range(0, parts.Length)];
        }
        public T GetRandomPerfectPart()
        {
            return perfectParts[Random.Range(0, perfectParts.Length)];
        }
    }


    private PartsByProgressionState<TurretPartProjectileDataModel>[] _attacksByProgressionStates;
    private PartsByProgressionState<TurretPartBody>[] _bodiesByProgressionStates;
    private PartsByProgressionState<ATurretPassiveAbilityDataModel>[] _passivesByProgressionStates;
    private PartsByProgressionState<TurretStatsUpgradeModel>[] _bonusStatsByProgressionStates;


    public void SetContent(AttackPartsLibraryContent newAttacksContent, BodyPartsLibraryContent newBodiesContent, 
        PassivesLibraryContent newPassivesContent, BonusStatsPartsLibraryContent newBonusStatsContent)
    {
        _attacksByProgressionStates = newAttacksContent.GetArrayByProgression();
        _bodiesByProgressionStates = newBodiesContent.GetArrayByProgression();
        _passivesByProgressionStates = newPassivesContent.GetArrayByProgression();
        _bonusStatsByProgressionStates = newBonusStatsContent.GetArrayByProgression();
    }


    // ATTACK PARTS
    private PartsByProgressionState<TurretPartProjectileDataModel> GetAttacksByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < _attacksByProgressionStates.Length; ++i)
        {
            if (_attacksByProgressionStates[i].progressionState == progressionState)
                return _attacksByProgressionStates[i];
        }

        return _attacksByProgressionStates[0];
    }

    public TurretPartProjectileDataModel[] GetRandomTurretPartAttacks(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, _attacksByProgressionStates.Length);
        PartsByProgressionState<TurretPartProjectileDataModel> attacksByProgressionState = GetAttacksByProgressionState(progressionState);
        HashSet<TurretPartProjectileDataModel> holderPartsSet = new HashSet<TurretPartProjectileDataModel>();

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
    private PartsByProgressionState<TurretPartBody> GetBodiesByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < _bodiesByProgressionStates.Length; ++i)
        {
            if (_bodiesByProgressionStates[i].progressionState == progressionState)
                return _bodiesByProgressionStates[i];
        }

        return _bodiesByProgressionStates[0];
    }

    public TurretPartBody[] GetRandomTurretPartBodies(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, _bodiesByProgressionStates.Length);
        PartsByProgressionState<TurretPartBody> bodiesByProgressionState = GetBodiesByProgressionState(progressionState);
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
    private PartsByProgressionState<ATurretPassiveAbilityDataModel> GetBasesAndPassivesByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < _passivesByProgressionStates.Length; ++i)
        {
            if (_passivesByProgressionStates[i].progressionState == progressionState)
                return _passivesByProgressionStates[i];
        }

        return _passivesByProgressionStates[0];
    }

    public ATurretPassiveAbilityDataModel[] GetRandomTurretPartBaseAndPassive(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, _passivesByProgressionStates.Length);
        PartsByProgressionState<ATurretPassiveAbilityDataModel> passiveByProgressionState = GetBasesAndPassivesByProgressionState(progressionState);
        HashSet<ATurretPassiveAbilityDataModel> holderPartsSet = new HashSet<ATurretPassiveAbilityDataModel>();

        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(passiveByProgressionState.GetRandomPerfectPart());
            }
        }

        while (holderPartsSet.Count < totalAmount)
        {
            holderPartsSet.Add(passiveByProgressionState.GetRandomPart());
        }


        return holderPartsSet.ToArray();
    }



    // BONUS STATS PARTS
    private PartsByProgressionState<TurretStatsUpgradeModel> GetBonusStatsByProgressionState(NodeEnums.ProgressionState progressionState)
    {
        for (int i = 0; i < _bonusStatsByProgressionStates.Length; ++i)
        {
            if (_bonusStatsByProgressionStates[i].progressionState == progressionState)
                return _bonusStatsByProgressionStates[i];
        }

        return _bonusStatsByProgressionStates[0];
    }

    public TurretStatsUpgradeModel[] GetRandomTurretStatsUpgradeModel(int totalAmount, int amountPerfect, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        totalAmount = Mathf.Min(totalAmount, _bonusStatsByProgressionStates.Length);
        PartsByProgressionState<TurretStatsUpgradeModel> bonusStatsByProgressionState = GetBonusStatsByProgressionState(progressionState);
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
