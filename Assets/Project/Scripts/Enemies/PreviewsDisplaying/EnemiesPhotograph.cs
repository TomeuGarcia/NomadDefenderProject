using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class EnemiesPhotograph : MonoBehaviour
{
    [SerializeField] private EnemyPhotoPersistent _enemyPhotoPersistent;
    [SerializeField] private RenderTexture _renderTexture;
    [SerializeField] private Camera _camera;
    [SerializeField] private AllEnemyTypeConfigsCollection _enemyTypesCollection;
    [SerializeField] private EnemyTypeConfig _fakeEnemyType;

    [SerializeField] private LineRenderer _gridLine;
    [SerializeField] private Transform _enemiesSpawnTransform;
    [SerializeField, Min(1)] private int _columns = 4;
    [SerializeField, Min(1)] private int _rows = 4;
    [SerializeField] private Vector2 _gridSize = new Vector2(2, 2);

    private List<Enemy> _enemies;

    public bool Finished { get; private set; }
    
    private IEnumerator Start()
    {
        Finished = false;
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
        _enemyPhotoPersistent.Init(_renderTexture);
        Finished = true;
    }

    private void Init()
    {
        _camera.enabled = false;
    }
    
    private void SpawnEnemies()
    {
        EnemyFactory enemyFactory = EnemyFactory.GetInstance();
        
        _enemies = new List<Enemy>(_enemyTypesCollection.EnemyTypes.Length);
        int photoIndexCounter = 0;
        
        for (int i = 0; i < _enemyTypesCollection.EnemyTypes.Length; ++i)
        {
            EnemyTypeConfig enemyTypeConfig = _enemyTypesCollection.EnemyTypes[i];
            if (enemyTypeConfig == _fakeEnemyType || enemyTypeConfig.IsArmored)
            {
                continue;
            }

            int row = (photoIndexCounter / _rows);
            int column = (photoIndexCounter % _columns);

            Vector3 position = LocalToWorldPosition(
                new Vector3((column + 0.5f) * _gridSize.x, (row + 0.5f) * _gridSize.y, 0));
            position += enemyTypeConfig.View.PhotoOffset;
            Quaternion rotation = enemyTypeConfig.View.PhotoRotation;

            Enemy enemy = enemyFactory.CreateEnemy(enemyTypeConfig, position, rotation, _enemiesSpawnTransform);
            
            enemy.InitWithoutFunctionality();

            enemyTypeConfig.View.PhotoIndex = photoIndexCounter;
            ++photoIndexCounter;
            
            
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
            enemy.MeshTransform.parent.localScale = enemy.MeshTransform.parent.localScale * enemy.TypeConfig.View.PhotoScale;
        }
    }
}
