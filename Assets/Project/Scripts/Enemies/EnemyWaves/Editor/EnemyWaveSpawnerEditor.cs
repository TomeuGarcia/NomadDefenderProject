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

        string color = enemyWaveSpawner.IsIncorrect ? "red" : "green";

        GUILayout.Label("\nJSON data");
        GUILayout.Label("Folder (Level) Name:   " + enemyWaveSpawner.NameLevel, color);
        GUILayout.Label("JSON Name:             " + enemyWaveSpawner.NameJSON, color);
        GUILayout.Label("Progression State:     " + enemyWaveSpawner.ProgressionState.ToString(), color);
        GUILayout.Label("Num nodes:             " + enemyWaveSpawner.NumNodes.ToString(), color);
        GUILayout.Label("Is Tutorial:           " + enemyWaveSpawner.IsTutorial.ToString(), color);
        //GUILayout.Label(enemyWaveSpawner.IsIncorrect);


    }

}
#endif