using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AllEnemyWaveSpawnersCollection), true)]
public class AllEnemyWaveSpawnersCollectionEditor : Editor
{
    private static readonly string[] _excludedProperties = { "m_Script" };

    private AllEnemyWaveSpawnersCollection _enemyWaveSpawnersCollectionInspected;

    private int _enemyWaveSpawnersTotal = 0;
    private GUIStyle _headerLabelStyle;


    private void OnEnable()
    {
        _enemyWaveSpawnersCollectionInspected = target as AllEnemyWaveSpawnersCollection;
        UpdateCollection();
        InitializeGuiStyles();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField($"Total EnemyWaveSpawners ({_enemyWaveSpawnersTotal}): ", _headerLabelStyle);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Update EnemyWaveSpawners"))
        {
            OnUpdateTextContentsPressed();
        }

        EditorGUILayout.Space(10);

        DrawPropertiesExcluding(serializedObject, _excludedProperties);
    }

    private void InitializeGuiStyles()
    {
        _headerLabelStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            fixedHeight = 40.0f
        };
    }


    private void OnUpdateTextContentsPressed()
    {
        UpdateCollection();
    }

    private void UpdateCollection()
    {
        Undo.RecordObject(_enemyWaveSpawnersCollectionInspected, "Update EnemyWaveSpawner Collection");

        EnemyWaveSpawner[] enemyWaveSpawners = FindAssetsHelper.GetAllAssetsInProject<EnemyWaveSpawner>();
        _enemyWaveSpawnersCollectionInspected.UpdateEnemyWaveSpawners(enemyWaveSpawners);

        _enemyWaveSpawnersTotal = enemyWaveSpawners.Length;

        EditorUtility.SetDirty(_enemyWaveSpawnersCollectionInspected);
    }

}
