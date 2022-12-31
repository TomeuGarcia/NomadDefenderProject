using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSceneLoader : MonoBehaviour
{
    [System.Serializable]
    struct SceneNames
    {
        public SceneNames(int numNames)
        {
            names = new List<string>(numNames);
        }

        public void Add(string name)
        {
            names.Add(name);
        }
        public string Get(int i)
        {
            return names[i];
        }
        public void RemoveAt(int i)
        {
            names.RemoveAt(i);
        }
        public int Length => names.Count;

        public List<string> names;
    }


    [Header("UPGRADE SCENES")]
    [SerializeField] private SceneNames upgradeScenes;

    [Header("BATTLE SCENES")]
    [SerializeField] private SceneNames[] earlyBattleScenes;
    [Space(50)]
    [SerializeField] private SceneNames[] lateBattleScenes;
    [Space(50)]
    [SerializeField] private SceneNames[] bossBattleScenes;
    [Space(50)]
    private SceneNames[] availableEarlyBattleScenes;
    private SceneNames[] availableLateBattleScenes;
    private SceneNames[] availableBossBattleScenes;


    public void Init()
    {
        CloneEntireSceneNames(out availableEarlyBattleScenes, earlyBattleScenes);
        CloneEntireSceneNames(out availableLateBattleScenes, lateBattleScenes);
        CloneEntireSceneNames(out availableBossBattleScenes, bossBattleScenes);
    }

    private void CloneEntireSceneNames(out SceneNames[] copy, SceneNames[] original)
    {
        copy = new SceneNames[original.Length];
        for (int i = 0; i < earlyBattleScenes.Length; ++i)
        {
            copy[i] = new SceneNames(original[i].Length);
            for (int j = 0; j < original[i].Length; ++j)
            {
                copy[i].Add(original[i].Get(j));
            }
        }
    }


    public void LoadUpgradeScene(NodeEnums.UpgradeType upgradeType, NodeEnums.HealthState nodeHealthState)
    {
        string sceneName = upgradeScenes.Get((int)upgradeType);

        Debug.Log("Loading Upgrade scene: " + sceneName);
    }

    public void LoadBattleScene(NodeEnums.BattleType battleType, int numLocationsToDefend)
    {
        SceneNames[] availableBattleScenes;
        SceneNames[] battleScenes;

        if (battleType == NodeEnums.BattleType.EARLY)
        {
            availableBattleScenes = availableEarlyBattleScenes;
            battleScenes = earlyBattleScenes;
        }
        else if (battleType == NodeEnums.BattleType.LATE)
        {
            availableBattleScenes = availableLateBattleScenes;
            battleScenes = lateBattleScenes;
        }
        else if (battleType == NodeEnums.BattleType.BOSS)
        {
            availableBattleScenes = availableBossBattleScenes;
            battleScenes = bossBattleScenes;
        }
        else // Default as Early
        {
            availableBattleScenes = availableEarlyBattleScenes;
            battleScenes = earlyBattleScenes;
        }


        int locationsAmountIndex = numLocationsToDefend - 1;

        int sceneNameIndex = Random.Range(0, availableBattleScenes[locationsAmountIndex].Length);
        string sceneName = availableBattleScenes[locationsAmountIndex].Get(sceneNameIndex);


        // Remove
        availableBattleScenes[locationsAmountIndex].RemoveAt(sceneNameIndex);

        // Refill
        if (availableBattleScenes[locationsAmountIndex].Length == 0)
        {
            for (int i = 0; i < battleScenes[locationsAmountIndex].Length; ++i)
            {
                availableBattleScenes[locationsAmountIndex].Add(battleScenes[locationsAmountIndex].Get(i));
            }
        }


        Debug.Log("Loading Battle scene: " + sceneName);
    }

}
