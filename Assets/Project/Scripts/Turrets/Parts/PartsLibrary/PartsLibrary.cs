using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartsLibrary", menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "TurretPartsLibrary")]
public class PartsLibrary : ScriptableObject
{
    private const int MAX_ITERATIONS = 10;
    
    
    [System.Serializable]
    public struct PartsByProgressionState<T>
    {
        public NodeEnums.ProgressionState progressionState;
        public T[] parts;
        //public T[] perfectParts;

        public T GetRandomPart()
        {
            return parts[Random.Range(0, parts.Length)];
        }
        /*
        public T GetRandomPerfectPart()
        {
            return perfectParts[Random.Range(0, perfectParts.Length)];
        }
        */
    }


    private PartsByProgressionState<TurretPartProjectileDataModel>[] _attacksByProgressionStates;
    private PartsByProgressionState<TurretPartBody>[] _bodiesByProgressionStates;
    private PartsByProgressionState<ATurretPassiveAbilityDataModel>[] _passivesByProgressionStates;
    private Dictionary<CardPartReplaceManager.BonusStatType, PartsByProgressionState<TurretStatsUpgradeModel>[]> _bonusStatsByProgressionStatesMap;


    public void SetContent(AttackPartsLibraryContent newAttacksContent, BodyPartsLibraryContent newBodiesContent, 
        PassivesLibraryContent newPassivesContent, BonusStatsPartsLibraryContent newBonusStatsContent)
    {
        _attacksByProgressionStates = newAttacksContent.GetArrayByProgression();
        _bodiesByProgressionStates = newBodiesContent.GetArrayByProgression();
        _passivesByProgressionStates = newPassivesContent.GetArrayByProgression();
        _bonusStatsByProgressionStatesMap = new Dictionary<CardPartReplaceManager.BonusStatType, PartsByProgressionState<TurretStatsUpgradeModel>[]>
        {
            { CardPartReplaceManager.BonusStatType.DAMAGE, newBonusStatsContent.GetArrayByProgression_Damage() },
            { CardPartReplaceManager.BonusStatType.SHOTS_PER_SECOND, newBonusStatsContent.GetArrayByProgression_ShotsPerSecond() },
            { CardPartReplaceManager.BonusStatType.RANGE, newBonusStatsContent.GetArrayByProgression_Range() },
        };
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

        /*
        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(attacksByProgressionState.GetRandomPerfectPart());
            }
        }
        */

        int iterations = 0;
        while (holderPartsSet.Count < totalAmount && iterations < MAX_ITERATIONS)
        {
            holderPartsSet.Add(attacksByProgressionState.GetRandomPart());
            ++iterations;
        }

        TurretPartProjectileDataModel[] projectilesInSet = holderPartsSet.ToArray();
        TurretPartProjectileDataModel[] projectiles = new TurretPartProjectileDataModel[totalAmount];
        for (int i = 0; i < projectilesInSet.Length; ++i)
        {
            projectiles[i] = projectilesInSet[i];
        }
        for (int i = projectilesInSet.Length; i < totalAmount; ++i)
        {
            projectiles[i] = attacksByProgressionState.GetRandomPart();
        }
        

        return projectiles;
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

        /*
        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(bodiesByProgressionState.GetRandomPerfectPart());
            }
        }
        */

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

        /*
        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(passiveByProgressionState.GetRandomPerfectPart());
            }
        }
        */

        int iterations = 0;
        while (holderPartsSet.Count < totalAmount && iterations < MAX_ITERATIONS)
        {
            holderPartsSet.Add(passiveByProgressionState.GetRandomPart());
            ++iterations;
        }

        ATurretPassiveAbilityDataModel[] passivesInSet = holderPartsSet.ToArray();
        ATurretPassiveAbilityDataModel[] passives = new ATurretPassiveAbilityDataModel[totalAmount];
        for (int i = 0; i < passivesInSet.Length; ++i)
        {
            passives[i] = passivesInSet[i];
        }
        for (int i = passivesInSet.Length; i < totalAmount; ++i)
        {
            passives[i] = passiveByProgressionState.GetRandomPart();
        }

        return passives;
    }



    // BONUS STATS PARTS
    private PartsByProgressionState<TurretStatsUpgradeModel> GetBonusStatsByProgressionState(NodeEnums.ProgressionState progressionState, 
        CardPartReplaceManager.BonusStatType bonusStatsTypes)
    {
        PartsByProgressionState<TurretStatsUpgradeModel>[] bonusStatsByProgressionStates = 
            _bonusStatsByProgressionStatesMap[bonusStatsTypes];
        
        for (int i = 0; i < bonusStatsByProgressionStates.Length; ++i)
        {
            if (bonusStatsByProgressionStates[i].progressionState == progressionState)
                return bonusStatsByProgressionStates[i];
        }

        return bonusStatsByProgressionStates[0];
    }

    public TurretStatsUpgradeModel[] GetRandomTurretStatsUpgradeModel(int totalAmount, int amountPerfect, bool perfect, 
        NodeEnums.ProgressionState progressionState, CardPartReplaceManager.BonusStatType bonusStatsTypes)
    {
        totalAmount = Mathf.Min(totalAmount, _bonusStatsByProgressionStatesMap[bonusStatsTypes].Length);
        PartsByProgressionState<TurretStatsUpgradeModel> bonusStatsByProgressionState = 
            GetBonusStatsByProgressionState(progressionState, bonusStatsTypes);
        HashSet<TurretStatsUpgradeModel> holderPartsSet = new HashSet<TurretStatsUpgradeModel>();

        /*
        if (perfect)
        {
            while (holderPartsSet.Count < amountPerfect)
            {
                holderPartsSet.Add(bonusStatsByProgressionState.GetRandomPerfectPart());
            }
        }
        */

        
        int iterations = 0;
        while (holderPartsSet.Count < totalAmount && iterations < MAX_ITERATIONS)
        {
            holderPartsSet.Add(bonusStatsByProgressionState.GetRandomPart());
            ++iterations;
        }

        TurretStatsUpgradeModel[] turretStatsInSet = holderPartsSet.ToArray();
        TurretStatsUpgradeModel[] turretStats = new TurretStatsUpgradeModel[totalAmount];
        for (int i = 0; i < turretStatsInSet.Length; ++i)
        {
            turretStats[i] = turretStatsInSet[i];
        }
        for (int i = turretStatsInSet.Length; i < totalAmount; ++i)
        {
            turretStats[i] = bonusStatsByProgressionState.GetRandomPart();
        }


        return turretStats;
    }
}
