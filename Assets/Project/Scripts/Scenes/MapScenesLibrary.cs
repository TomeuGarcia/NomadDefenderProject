using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMapScenesLibrary", 
    menuName = SOAssetPaths.MAP_SCENES + "MapScenesLibrary")]
public class MapScenesLibrary : ScriptableObject
{
    [Header("UPGRADE SCENE'S NAMES")]
    [SerializeField] private string newTurretCard;
    [SerializeField] private string replaceAttackPart;
    [SerializeField] private string replaceBodyPart;
    [SerializeField] private string replaceBasePart;
    [SerializeField] private string addBonusStatsPart;


    [Header("\n\nBATTLE SCENE'S NAMES")]
    [Header("EARLY")]
    [SerializeField] private string[] early1Defense;
    [SerializeField] private string[] early2Defense;

    [Header("MID")]
    [SerializeField] private string[] mid1Defense;
    [SerializeField] private string[] mid2Defenses;

    [Header("LATE")]
    [SerializeField] private string[] late1Defense;
    [SerializeField] private string[] late2Defenses;

    [Header("BOSS")]
    [SerializeField] private string[] boss1Defense;
    [SerializeField] private string[] boss2Defenses;


    [Header("\n\nTESTING: Battle scene's names")]
    [SerializeField] private bool useTestingScenes = false;
    [SerializeField] private string[] testing1Defense;
    [SerializeField] private string[] testing2Defenses;


    public void SetUpgradeSceneNames(out MapSceneLoader.SceneNames sceneNames)
    {
        int n = (int)NodeEnums.UpgradeType.COUNT;

        string[] sceneNamesArray = new string[n];
        sceneNamesArray[(int)NodeEnums.UpgradeType.NEW_TURRET_CARD] = newTurretCard;
        sceneNamesArray[(int)NodeEnums.UpgradeType.REPLACE_ATTACK_PART] = replaceAttackPart;
        sceneNamesArray[(int)NodeEnums.UpgradeType.REPLACE_BODY_PART] = replaceBodyPart;
        sceneNamesArray[(int)NodeEnums.UpgradeType.REPLACE_BASE_PART] = replaceBasePart;
        sceneNamesArray[(int)NodeEnums.UpgradeType.ADD_BONUS_STATS_PART] = addBonusStatsPart;

        sceneNames = new MapSceneLoader.SceneNames(sceneNamesArray);
    }


    public void SetBattleSceneNames(out MapSceneLoader.SceneNames[] earlySceneNames,
                                    out MapSceneLoader.SceneNames[] midSceneNames,
                                    out MapSceneLoader.SceneNames[] lateSceneNames,
                                    out MapSceneLoader.SceneNames[] bossSceneNames)
    {
        if (useTestingScenes)
        {
            SetBattleTestingSceneNames(out earlySceneNames, out midSceneNames, out lateSceneNames, out bossSceneNames);
            return;
        }

        earlySceneNames = new MapSceneLoader.SceneNames[2];
        earlySceneNames[0] = new MapSceneLoader.SceneNames(early1Defense);
        earlySceneNames[1] = new MapSceneLoader.SceneNames(early2Defense);

        midSceneNames = new MapSceneLoader.SceneNames[2];
        midSceneNames[0] = new MapSceneLoader.SceneNames(mid1Defense);
        midSceneNames[1] = new MapSceneLoader.SceneNames(mid2Defenses);

        lateSceneNames = new MapSceneLoader.SceneNames[2];
        lateSceneNames[0] = new MapSceneLoader.SceneNames(late1Defense);
        lateSceneNames[1] = new MapSceneLoader.SceneNames(late2Defenses);

        bossSceneNames = new MapSceneLoader.SceneNames[2];
        bossSceneNames[0] = new MapSceneLoader.SceneNames(boss1Defense);
        bossSceneNames[1] = new MapSceneLoader.SceneNames(boss2Defenses);
    }

    private void SetBattleTestingSceneNames(out MapSceneLoader.SceneNames[] earlySceneNames,
                                            out MapSceneLoader.SceneNames[] midSceneNames,
                                            out MapSceneLoader.SceneNames[] lateSceneNames,
                                            out MapSceneLoader.SceneNames[] bossSceneNames)
    {
        earlySceneNames = new MapSceneLoader.SceneNames[2];
        earlySceneNames[0] = new MapSceneLoader.SceneNames(testing1Defense);
        earlySceneNames[1] = new MapSceneLoader.SceneNames(testing2Defenses);

        midSceneNames = new MapSceneLoader.SceneNames[2];
        midSceneNames[0] = new MapSceneLoader.SceneNames(testing1Defense);
        midSceneNames[1] = new MapSceneLoader.SceneNames(testing2Defenses);

        lateSceneNames = new MapSceneLoader.SceneNames[2];
        lateSceneNames[0] = new MapSceneLoader.SceneNames(testing1Defense);
        lateSceneNames[1] = new MapSceneLoader.SceneNames(testing2Defenses);

        bossSceneNames = new MapSceneLoader.SceneNames[2];
        bossSceneNames[0] = new MapSceneLoader.SceneNames(testing1Defense);
        bossSceneNames[1] = new MapSceneLoader.SceneNames(testing2Defenses);
    }

}