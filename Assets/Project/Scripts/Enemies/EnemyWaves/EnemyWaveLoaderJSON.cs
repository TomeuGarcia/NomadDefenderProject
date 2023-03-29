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

        public EnemyWavesWrapper()
        {
            enemyWaves = new EnemyWave[2];
            for (int i = 0; i < enemyWaves.Length; ++i)
            {
                enemyWaves[i] = new EnemyWave();
            }
        }
        public EnemyWavesWrapper(EnemyWave[] enemyWaves)
        {
            this.enemyWaves = new EnemyWave[enemyWaves.Length];
            enemyWaves.CopyTo(this.enemyWaves, 0);
        }

    }


    public static void SaveEnemyWave(EnemyWaveSpawner enemyWaveSpawner, bool usePersistentDirectoryPath)
    {
        string path = GetPathToFileJSON(enemyWaveSpawner, usePersistentDirectoryPath);

        if (enemyWaveSpawner.NameLevel.Length < 2) return;
        if (enemyWaveSpawner.NameLevel[2] != '_') return; // Scuffed, please remove when fixed

        Debug.Log(path);


        EnemyWavesWrapper enemyWavesWrapper = new EnemyWavesWrapper(enemyWaveSpawner.EnemyWaves);

        WriteEnemyWavesToJSON(enemyWavesWrapper, path);
    }

    private static void WriteEnemyWavesToJSON(EnemyWavesWrapper enemyWavesWrapper, string path)
    {
        string content = JsonUtility.ToJson(enemyWavesWrapper);

        File.WriteAllText(path, content);
    }


    public static void LoadEnemyWave(EnemyWaveSpawner enemyWaveSpawner, bool usePersistentDirectoryPath)
    {
        string path = GetPathToFileJSON(enemyWaveSpawner, usePersistentDirectoryPath);
        string content = File.ReadAllText(path);

        EnemyWavesWrapper enemyWavesWrapper = JsonUtility.FromJson<EnemyWavesWrapper>(content);
        enemyWaveSpawner.SetEnemyWaves(enemyWavesWrapper.enemyWaves);        
    }


    public static string GetPathToDirectoryJSON(EnemyWaveSpawner enemyWaveSpawner, bool usePersistentDirectoryPath)
    {
        string pathToJSON = EnemyWaveSpawner.PATH_TO_JSON;
        if (enemyWaveSpawner.IsIncorrect) pathToJSON += "INCORRECT/";
        if (enemyWaveSpawner.IsTutorial) pathToJSON += "Tutorials/";

        string progressionStatePath = EnemyWaveSpawner.PROGRESSION_PATHS[(int)enemyWaveSpawner.ProgressionState];
        string numNodesPath = EnemyWaveSpawner.NUM_NODES_PATHS[enemyWaveSpawner.NumNodes - 1];
        string levelName = enemyWaveSpawner.NameLevel + "/";

        string rootDirectory = usePersistentDirectoryPath ? Application.persistentDataPath : Application.streamingAssetsPath;
        
        string directory = rootDirectory + pathToJSON + progressionStatePath + numNodesPath + levelName;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return directory;
    }

    public static string GetJSONFileName(EnemyWaveSpawner enemyWaveSpawner)
    {
        return enemyWaveSpawner.NameJSON + ".json";
    }

    public static string GetPathToFileJSON(EnemyWaveSpawner enemyWaveSpawner, bool usePersistentDirectoryPath)
    {
        return GetPathToFileJSON(GetPathToDirectoryJSON(enemyWaveSpawner, usePersistentDirectoryPath), enemyWaveSpawner);
    }
    public static string GetPathToFileJSON(string directoryPath, EnemyWaveSpawner enemyWaveSpawner)
    {
        string targetFilePath = GetJSONFileName(enemyWaveSpawner);

        Debug.Log(directoryPath + targetFilePath);

        return directoryPath + targetFilePath;
    }


    public static void GenerateNewEmptyJSON(EnemyWaveSpawner enemyWaveSpawner, bool usePersistentDirectoryPath)
    {
        string directory = GetPathToDirectoryJSON(enemyWaveSpawner, usePersistentDirectoryPath);
        string targetFilePath = enemyWaveSpawner.NameJSON + ".json";
        string path = directory + targetFilePath;


        EnemyWavesWrapper enemyWavesWrapper = new EnemyWavesWrapper();

        WriteEnemyWavesToJSON(enemyWavesWrapper, path);
    }


}
