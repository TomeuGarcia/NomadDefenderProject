using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyWaveSpawner))]
[CanEditMultipleObjects]
public class EnemyWaveSpawnerEditor : Editor
{
    private bool firstTime = true;
    private bool jsonExists = false;
    private EnemyWaveSpawner _enemyWaveSpawnerInspected;

    private GUIStyle _headerLabelStyle;
    private GUIStyle _header2LabelStyle;

    private void OnEnable()
    {
        InitializeGuiStyles();
        _enemyWaveSpawnerInspected = target as EnemyWaveSpawner;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _enemyWaveSpawnerInspected.ValidateJSONFormat();


        if (firstTime)
        {
            firstTime = false;

            jsonExists = EnemyWaveJSONManager.DoesJSONExist(_enemyWaveSpawnerInspected);
        }

        DisplayDroppedCurrency();
        //DisplayInfoEnemyWaveSpawnerJSON(_enemyWaveSpawnerInspected);
        //DisplayJSONButtons();
        //PrintEnemyTypeLegend();
    }


    private void InitializeGuiStyles()
    {
        _headerLabelStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            fixedHeight = 40.0f
        };
        _headerLabelStyle.normal.textColor = Color.white;

        _header2LabelStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            fixedHeight = 30.0f
        };
    }

    private void DisplayDroppedCurrency()
    {
        EditorGUILayout.LabelField($"Currency Dropped (by wave index)", _headerLabelStyle);
        int totalWavesCurrency = 0;
        for (int i = 0; i < _enemyWaveSpawnerInspected.EnemyWaves.Length; ++i)
        {
            EnemyWave enemyWave = _enemyWaveSpawnerInspected.EnemyWaves[i];
            int waveCurrency = 0;
            foreach (EnemyInWave enemyInWave in enemyWave.enemiesInWave)
            {
                waveCurrency += enemyInWave.EnemyTypeN.BaseStats.CurrencyDrop;
            }

            EditorGUILayout.LabelField($" {i}. {waveCurrency} ", _header2LabelStyle);
            totalWavesCurrency += waveCurrency;

        }

        EditorGUILayout.LabelField($" Total: {totalWavesCurrency} ", _header2LabelStyle);
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

    private void DisplayJSONButtons()
    {
        if (_enemyWaveSpawnerInspected.IsIncorrect) return;

        if (jsonExists)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Show JSON In Folder", GUILayout.Width(200), GUILayout.Height(30)))
            {
                ShowJSONInFolder(_enemyWaveSpawnerInspected);
            }
        }
        else
        {
            GUI.color = Color.green;
            GUILayout.Space(20);
            if (GUILayout.Button("Generate New Empty JSON", GUILayout.Width(200), GUILayout.Height(30)))
            {
                GenerateNewEmptyJSON(_enemyWaveSpawnerInspected);
                jsonExists = true;
            }
            GUI.color = Color.white;
        }

        if (jsonExists)
        {
            GUILayout.Space(40);
            if (GUILayout.Button("Load data from JSON (DEBUG & JSON TEST ONLY)", GUILayout.Width(350), GUILayout.Height(30)))
            {
                EnemyWaveJSONManager.LoadEnemyWave(_enemyWaveSpawnerInspected, false);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Save data to JSON (DATA WILL BE OVERWRITTEN)", GUILayout.Width(350), GUILayout.Height(30)))
            {
                EnemyWaveJSONManager.SaveEnemyWave(_enemyWaveSpawnerInspected, false);
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
