using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class EnemiesPhotograph : MonoBehaviour
{
    [SerializeField] private RenderTexture _renderTexture;
    [SerializeField] private Camera _camera;
    [SerializeField] private AllEnemyTypeConfigsCollection _enemyTypesCollection;
    [SerializeField] private EnemyTypeConfig _fakeEnemyType;

    [SerializeField] private LineRenderer _gridLine;
    [SerializeField] private Transform _enemiesSpawnTransform;
    [SerializeField, Min(1)] private int _columns = 4;
    [SerializeField] private Vector2 _gridSize = new Vector2(2, 2);
    private int _rows;

    private List<Enemy> _enemies;

    private IEnumerator Start()
    {
        Init();
        SpawnEnemies();
        DrawGrid();
        yield return new WaitForEndOfFrame();
        FixEnemiesPosition();
        _camera.targetTexture = _renderTexture;
        _camera.Render();
        _camera.targetTexture = null;
        yield return null;
        yield return null;
    }

    private void Init()
    {
        _rows = Mathf.CeilToInt((float)_enemyTypesCollection.EnemyTypes.Length / _columns);
        _camera.enabled = false;
    }
    
    private void SpawnEnemies()
    {
        EnemyFactory enemyFactory = EnemyFactory.GetInstance();
        
        _enemies = new List<Enemy>(_enemyTypesCollection.EnemyTypes.Length);
        
        for (int i = 0; i < _enemyTypesCollection.EnemyTypes.Length; ++i)
        {
            EnemyTypeConfig enemyTypeConfig = _enemyTypesCollection.EnemyTypes[i];
            if (enemyTypeConfig == _fakeEnemyType)
            {
                continue;
            }
            
            
            int row = (i / _rows);
            int column = (i % _columns);

            Vector3 position = LocalToWorldPosition(
                new Vector3((column + 0.5f) * _gridSize.x, (row + 0.5f) * _gridSize.y, 0));
            Quaternion rotation = enemyTypeConfig.View.PhotoRotation;

            GameObject enemyGameObject = 
                enemyFactory.GetEnemyGameObject(enemyTypeConfig, position, rotation, _enemiesSpawnTransform);
            enemyGameObject.SetActive(true);
            
            Enemy enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.InitWithoutFunctionality();

            enemyTypeConfig.View.PhotoIndex = i;
            
            
            _enemies.Add(enemy);
        }
    }

    private void DrawGrid()
    {
        int numberOfPoints = (_rows * _columns * 5) + _rows;
        _gridLine.positionCount = numberOfPoints;

        int index = 0;
        for (int row = 0; row < _rows; ++row)
        {
            for (int column = 0; column < _columns; ++column)
            {
                Vector3 positionTopLeft     = LocalToWorldPosition(new Vector3(_gridSize.x * (column + 0), _gridSize.y * (row + 0), 0));
                Vector3 positionTopRight    = LocalToWorldPosition(new Vector3(_gridSize.x * (column + 1), _gridSize.y * (row + 0), 0));
                Vector3 positionBottomRight = LocalToWorldPosition(new Vector3(_gridSize.x * (column + 1), _gridSize.y * (row + 1), 0));
                Vector3 positionBottomLeft  = LocalToWorldPosition(new Vector3(_gridSize.x * (column + 0), _gridSize.y * (row + 1), 0));
                
                _gridLine.SetPosition(index + 0, positionTopLeft);
                _gridLine.SetPosition(index + 1, positionTopRight);
                _gridLine.SetPosition(index + 2, positionBottomRight);
                _gridLine.SetPosition(index + 3, positionBottomLeft);
                _gridLine.SetPosition(index + 4, positionTopLeft);
                
                index += 5;
            }
            
            Vector3 positionRowStart = LocalToWorldPosition(new Vector3(0, _gridSize.y * row, 0)); 
            _gridLine.SetPosition(index + 0, positionRowStart);
            index += 1;
        }
    }

    private Vector3 LocalToWorldPosition(Vector3 localPosition)
    {
        return _enemiesSpawnTransform.position + localPosition;
    }


    [Button()]
    private void FixEnemiesPosition()
    {
        for (int i = 0; i < _enemies.Count; ++i)
        {
            Enemy enemy = _enemies[i];
            enemy.PositionWithCenteredMesh();
            enemy.MeshTransform.localScale = enemy.MeshTransform.localScale * enemy.TypeConfig.View.PhotoScale;
        }
    }
}
