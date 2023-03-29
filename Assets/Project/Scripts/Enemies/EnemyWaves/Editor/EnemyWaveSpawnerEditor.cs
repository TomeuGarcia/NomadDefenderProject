using System.Collections;
using System.Collections.Generic;

using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemyWaveSpawner))]
[CanEditMultipleObjects]
public class EnemyWaveSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyWaveSpawner enemyWaveSpawner = target as EnemyWaveSpawner;

        DisplayInfoEnemyWaveSpawnerJSON(enemyWaveSpawner);

        if (GUILayout.Button("Show Folder"))
        {
            ShowFolder(enemyWaveSpawner);
        }
    }


    private void DisplayInfoEnemyWaveSpawnerJSON(EnemyWaveSpawner enemyWaveSpawner)
    {
        string[] content = { "yep", "pepe", "yep", "pepe" };
        
        GUILayout.SelectionGrid(-1, content, 2);
        GUILayout.Toolbar(-1, content);
        GUILayout.Toolbar(-1, content);

        GUILayout.Label("\nJSON data");
        GUILayout.Label("Folder (Level) Name:   " + enemyWaveSpawner.NameLevel);
        GUILayout.Label("JSON Name:             " + enemyWaveSpawner.NameJSON);
        GUILayout.Label("Progression State:     " + enemyWaveSpawner.ProgressionState.ToString());
        GUILayout.Label("Num nodes:             " + enemyWaveSpawner.NumNodes.ToString());
        GUILayout.Label("Is Tutorial:           " + enemyWaveSpawner.IsTutorial.ToString());
        //GUILayout.Label(enemyWaveSpawner.IsIncorrect);
    }

    private void ShowFolder(EnemyWaveSpawner enemyWaveSpawner)
    {
        string pathToDirectory = EnemyWaveJSONManager.GetPathToDirectoryJSON(enemyWaveSpawner);
        EditorUtility.RevealInFinder(pathToDirectory);
    }


}
#endif