using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapSceneLoader;

public class MapSceneLoader : MonoBehaviour
{
    [System.Serializable]
    public struct SceneNames
    {
        public SceneNames(int numNames)
        {
            names = new List<string>(numNames);
        }
        public SceneNames(string[] namesArray)
        {
            names = new List<string>(namesArray);
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

    [Header("MAP SCENES LIBRARY")]
    [SerializeField] private MapScenesLibrary mapScenesLibrary;


    private SceneNames upgradeScenes;

    private SceneNames[] earlyBattleScenes;
    private SceneNames[] midBattleScenes;
    private SceneNames[] lateBattleScenes;
    private SceneNames[] bossBattleScenes;
    private SceneNames[] availableEarlyBattleScenes;
    private SceneNames[] availableMidBattleScenes;
    private SceneNames[] availableLateBattleScenes;
    private SceneNames[] availableBossBattleScenes;

    private string currentSceneName;

    public delegate void MapSceneLoaderAction();
    public event MapSceneLoaderAction OnSceneFromMapLoaded;
    public event MapSceneLoaderAction OnSceneFromMapUnloaded;



    public void Init()
    {
        mapScenesLibrary.SetUpgradeSceneNames(out upgradeScenes);
        mapScenesLibrary.SetBattleSceneNames(out earlyBattleScenes, out midBattleScenes, out lateBattleScenes, out bossBattleScenes);

        CloneEntireSceneNames(out availableEarlyBattleScenes, earlyBattleScenes);
        CloneEntireSceneNames(out availableMidBattleScenes, midBattleScenes);
        CloneEntireSceneNames(out availableLateBattleScenes, lateBattleScenes);
        CloneEntireSceneNames(out availableBossBattleScenes, bossBattleScenes);
    }

    private void CloneEntireSceneNames(out SceneNames[] copy, SceneNames[] original)
    {
        copy = new SceneNames[original.Length];
        for (int i = 0; i < original.Length; ++i)
        {
            copy[i] = new SceneNames(original[i].Length);
            for (int j = 0; j < original[i].Length; ++j)
            {
                copy[i].Add(original[i].Get(j));
            }
        }
    }

    public void LoadMainMenuScene(float delay)
    {
        StartCoroutine(DoLoadMainMenuScene(delay));
    }
    private IEnumerator DoLoadMainMenuScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneLoader.GetInstance().StartLoadMainMenu();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 1f);
    }


    public void LoadUpgradeScene(NodeEnums.UpgradeType upgradeType, NodeEnums.HealthState nodeHealthState)
    {
        string sceneName = upgradeScenes.Get((int)upgradeType);

        //Debug.Log("Loading Upgrade scene: " + sceneName);
        StartScene(sceneName);
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
        else if (battleType == NodeEnums.BattleType.MID)
        {
            availableBattleScenes = availableMidBattleScenes;
            battleScenes = midBattleScenes;
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
            //Debug.Log("Refilling battle scenes");
        }


        //Debug.Log("Loading Battle scene: " + sceneName);
        StartScene(sceneName);
    }

    public void LoadTutorialScene()
    {
        currentSceneName = "InBattleTutorial";
        SceneLoader.GetInstance().LoadMapScene(currentSceneName, false);

        SceneManager.sceneLoaded += InvokeOnSceneFromMapLoaded;
    }


    private void StartScene(string sceneName)
    {
        currentSceneName = sceneName;
        SceneLoader.GetInstance().LoadMapScene(sceneName, true);

        SceneManager.sceneLoaded += InvokeOnSceneFromMapLoaded;
    }

    public void FinishCurrentScene()
    {
        SceneLoader.GetInstance().UnloadMapScene(currentSceneName, true);

        SceneManager.sceneUnloaded += InvokeOnSceneFromMapUnloaded;
    }

    private void InvokeOnSceneFromMapLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (OnSceneFromMapLoaded != null) OnSceneFromMapLoaded();

        SceneManager.sceneLoaded -= InvokeOnSceneFromMapLoaded;
    }
    private void InvokeOnSceneFromMapUnloaded(Scene scene)
    {
        if (OnSceneFromMapUnloaded != null) OnSceneFromMapUnloaded();

        SceneManager.sceneUnloaded -= InvokeOnSceneFromMapUnloaded;
    }

}
