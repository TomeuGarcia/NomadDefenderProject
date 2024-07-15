using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AllEnemyTypeConfigsCollection), true)]
public class AllEnemyTypeConfigsCollectionEditor : Editor
{
    private static readonly string[] _excludedProperties = { "m_Script" };

    private AllEnemyTypeConfigsCollection _enemyTypesCollectionInspected;

    private int _enemyTypesTotal = 0;
    private GUIStyle _headerLabelStyle;


    private void OnEnable()
    {
        _enemyTypesCollectionInspected = target as AllEnemyTypeConfigsCollection;
        UpdateCollection();
        InitializeGuiStyles();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField($"Total EnemyTypes ({_enemyTypesTotal}): ", _headerLabelStyle);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Update EnemyTypes"))
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
        Undo.RecordObject(_enemyTypesCollectionInspected, "Update EnemyTypes Collection");

        EnemyTypeConfig[] enemyTypes = FindAssetsHelper.GetAllAssetsInProject<EnemyTypeConfig>();
        _enemyTypesCollectionInspected.UpdateEnemyTypes(enemyTypes);

        _enemyTypesTotal = enemyTypes.Length;

        EditorUtility.SetDirty(_enemyTypesCollectionInspected);
    }


}
