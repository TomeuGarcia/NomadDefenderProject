using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemyWaveSpawner))]
[CanEditMultipleObjects]
public class EnemyWaveSpawnerEditor : Editor
{
    private bool firstTime = true;
    private bool jsonExists = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyWaveSpawner enemyWaveSpawner = target as EnemyWaveSpawner;
        enemyWaveSpawner.ValidateJSONFormat();


        if (firstTime)
        {
            firstTime = false;

            jsonExists = EnemyWaveJSONManager.DoesJSONExist(enemyWaveSpawner);
        }



        DisplayInfoEnemyWaveSpawnerJSON(enemyWaveSpawner);

        if (enemyWaveSpawner.IsIncorrect) return;

        if (jsonExists)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Show JSON In Folder", GUILayout.Width(200), GUILayout.Height(30)))
            {
                ShowJSONInFolder(enemyWaveSpawner);
            }
        }
        else
        {
            GUI.color = Color.green;
            GUILayout.Space(20);
            if (GUILayout.Button("Generate New Empty JSON", GUILayout.Width(200), GUILayout.Height(30)))
            {
                GenerateNewEmptyJSON(enemyWaveSpawner);
                jsonExists = true;
            }
            GUI.color = Color.white;
        }

        if (jsonExists)
        {
            GUILayout.Space(40);
            if (GUILayout.Button("Load data from JSON (DEBUG & JSON TEST ONLY)", GUILayout.Width(350), GUILayout.Height(30)))
            {
                EnemyWaveJSONManager.LoadEnemyWave(enemyWaveSpawner, false);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Save data to JSON (DATA WILL BE OVERWRITTEN)", GUILayout.Width(350), GUILayout.Height(30)))
            {
                EnemyWaveJSONManager.SaveEnemyWave(enemyWaveSpawner, false);
            }
        }

        //GUILayout.Space(40);
        //if (GUILayout.Button("Save to Workaround", GUILayout.Width(200), GUILayout.Height(30)))
        //{
        //    enemyWaveSpawner.SaveToWorkaround();
        //}
        //GUILayout.Space(20);
        //if (GUILayout.Button("Load from Workaround", GUILayout.Width(200), GUILayout.Height(30)))
        //{
        //    enemyWaveSpawner.LoadFromWorkaround();
        //}


        PrintEnemyTypeLegend();
    }


    private void DisplayInfoEnemyWaveSpawnerJSON(EnemyWaveSpawner enemyWaveSpawner)
    {
        GUILayout.Space(20);       
        
        if (enemyWaveSpawner.IsIncorrect)
        {
            GUI.color = Color.red;
            GUILayout.Label("Incorrect EnemyWaveSpawer Name");
            GUI.color = Color.white;

            GUI.color = Color.cyan;
            GUILayout.Label("NAMING GUIDE:");
            GUI.color = Color.white;
            GUILayout.Label("first char --> E (Early), M (Mid), L (Late)");
            GUILayout.Label("second char --> 1 (1 Node), 2 (2 Nodes)");
            GUILayout.Label("follow with --> _PepeSmile (Level Name)");
            GUILayout.Label("finish with --> _Enemies1 (Spawner n1), _Enemies2 (Spawner n2), etc.");
            GUILayout.Space(2);
            GUI.color = Color.cyan;
            GUILayout.Label("EXAMPLE --> E1_LevelName_Enemies1");
            GUILayout.Label("                        (Early, 1 Node, Spawner n1)");
            GUI.color = Color.white;
            GUILayout.Space(20);
        }


        GUILayout.Label("JSON data:");
        GUILayout.Space(4);
        GUILayout.Label("Folder (Level) Name --> " + enemyWaveSpawner.NameLevel);
        GUILayout.Label("JSON Name --> " + enemyWaveSpawner.NameJSON);
        GUILayout.Label("Progression State --> " + enemyWaveSpawner.ProgressionState.ToString());
        GUILayout.Label("Num nodes --> " + enemyWaveSpawner.NumNodes.ToString());
        GUILayout.Label("Is Tutorial --> " + enemyWaveSpawner.IsTutorial.ToString());      
    }

    private void ShowJSONInFolder(EnemyWaveSpawner enemyWaveSpawner)
    {
        string pathToDirectory = EnemyWaveJSONManager.GetPathToDirectoryJSON(enemyWaveSpawner, false);

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
        EnemyWaveJSONManager.GenerateNewEmptyJSON(enemyWaveSpawner, false);
        ShowJSONInFolder(enemyWaveSpawner);
    }


    private void PrintEnemyTypeLegend()
    {
        GUILayout.Space(20);

        GUI.color = Color.yellow;
        GUILayout.Label("ENEMY TYPE LEGEND");
        GUI.color = Color.white;

        for (int enemyTypeI = 0; enemyTypeI < (int)Enemy.EnemyType.COUNT; ++enemyTypeI)
        {
            GUILayout.Label(((Enemy.EnemyType)enemyTypeI).ToString() + " = " + enemyTypeI.ToString());
        }
    }

}
#endif