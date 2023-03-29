using NodeEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class EnemyWaveJSONManager
{
    [System.Serializable]
    private class EnemyWavesWrapper
    {
        [SerializeField] public EnemyWave[] enemyWaves;

        public EnemyWavesWrapper(EnemyWave[] enemyWaves)
        {
            this.enemyWaves = new EnemyWave[enemyWaves.Length];
            enemyWaves.CopyTo(this.enemyWaves, 0);
        }

    }


    public static void SaveEnemyWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        string path = GetPathToFileJSON(enemyWaveSpawner);

        if (enemyWaveSpawner.NameLevel.Length < 2) return;
        if (enemyWaveSpawner.NameLevel[2] != '_') return; // Scuffed, please remove when fixed

        Debug.Log(path);

        EnemyWavesWrapper enemyWavesWrapper = new EnemyWavesWrapper(enemyWaveSpawner.EnemyWaves);
        string content = JsonUtility.ToJson(enemyWavesWrapper);

        File.WriteAllText(path, content);
    }


    public static void LoadEnemyWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        string path = GetPathToFileJSON(enemyWaveSpawner);
        string content = File.ReadAllText(path);

        EnemyWavesWrapper enemyWavesWrapper = JsonUtility.FromJson<EnemyWavesWrapper>(content);
        enemyWaveSpawner.SetEnemyWaves(enemyWavesWrapper.enemyWaves);        
    }


    public static string GetPathToDirectoryJSON(EnemyWaveSpawner enemyWaveSpawner)
    {
        string pathToJSON = EnemyWaveSpawner.PATH_TO_JSON;
        if (enemyWaveSpawner.IsIncorrect) pathToJSON += "INCORRECT/";
        if (enemyWaveSpawner.IsTutorial) pathToJSON += "Tutorials/";

        string progressionStatePath = EnemyWaveSpawner.PROGRESSION_PATHS[(int)enemyWaveSpawner.ProgressionState];
        string numNodesPath = EnemyWaveSpawner.NUM_NODES_PATHS[enemyWaveSpawner.NumNodes - 1];
        string levelName = enemyWaveSpawner.NameLevel + "/";

        string directory = Application.persistentDataPath + pathToJSON + progressionStatePath + numNodesPath + levelName;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return directory;
    }

    public static string GetPathToFileJSON(EnemyWaveSpawner enemyWaveSpawner)
    {
        string targetFilePath = enemyWaveSpawner.NameJSON + ".json";

        return GetPathToDirectoryJSON(enemyWaveSpawner) + targetFilePath;
    }


    
}
