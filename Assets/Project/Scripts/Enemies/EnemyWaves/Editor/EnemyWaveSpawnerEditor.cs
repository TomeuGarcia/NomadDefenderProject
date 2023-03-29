using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemyWaveSpawner))]
[CanEditMultipleObjects]
public class EnemyWaveSpawnerEditor : Editor
{

    public static string[] PROGRESSION_PATHS = { "EarlyLevels/", "MidLevels/", "LateLevels/" };
    public static string[] NUM_NODES_PATHS = { "1Node/", "2Nodes/" };
    public const string PATH_TO_JSON = "/JSONfiles/EnemyWaves/";


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyWaveSpawner enemyWaveSpawner = target as EnemyWaveSpawner;
        enemyWaveSpawner.ValidateJSONFormat();

        DisplayInfoEnemyWaveSpawnerJSON(enemyWaveSpawner);



        GUILayout.Space(20);
        if (GUILayout.Button("Show JSON In Folder", GUILayout.Width(200), GUILayout.Height(30)))
        {
            ShowJSONInFolder(enemyWaveSpawner);
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Generate New Empty JSON", GUILayout.Width(200), GUILayout.Height(30)))
        {
            GenerateNewEmptyJSON(enemyWaveSpawner);
        }
    }


    private void DisplayInfoEnemyWaveSpawnerJSON(EnemyWaveSpawner enemyWaveSpawner)
    {
        GUILayout.Space(20);       
        
        GUILayout.Label("JSON data");
        if (enemyWaveSpawner.IsIncorrect)
        {
            GUI.color = Color.red;
            GUILayout.Label("Incorrect EnemyWaveSpawer Name");
            GUI.color = Color.white;
        }

        GUILayout.Space(4);

        GUILayout.Label("Folder (Level) Name: " + enemyWaveSpawner.NameLevel);
        GUILayout.Label("JSON Name: " + enemyWaveSpawner.NameJSON);
        GUILayout.Label("Progression State: " + enemyWaveSpawner.ProgressionState.ToString());
        GUILayout.Label("Num nodes: " + enemyWaveSpawner.NumNodes.ToString());
        GUILayout.Label("Is Tutorial: " + enemyWaveSpawner.IsTutorial.ToString());      
    }

    private void ShowJSONInFolder(EnemyWaveSpawner enemyWaveSpawner)
    {
        string pathToDirectory = EnemyWaveJSONManager.GetPathToDirectoryJSON(enemyWaveSpawner, true);

        if (Directory.Exists(pathToDirectory))
        {
            EditorUtility.RevealInFinder(EnemyWaveJSONManager.GetPathToFileJSON(pathToDirectory, enemyWaveSpawner));
        }
        else
        {
            Debug.Log("DIRECTORY DOESNT EXIST");
        }
        
    }

    private void GenerateNewEmptyJSON(EnemyWaveSpawner enemyWaveSpawner)
    {
        EnemyWaveJSONManager.GenerateNewEmptyJSON(enemyWaveSpawner, true);
        ShowJSONInFolder(enemyWaveSpawner);
    }

}
#endif