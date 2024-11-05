using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesPhotograph : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private AllEnemyTypeConfigsCollection _enemyTypesCollection;
    [SerializeField] private EnemyTypeConfig _fakeEnemyType;


    [SerializeField] private Transform _enemiesSpawnTransform;
    [SerializeField, Min(1)] private int _columns = 4;
    [SerializeField] private Vector2 _gridSize = new Vector2(2, 2);


    private IEnumerator Start()
    {
        SpawnEnemies();
        yield return new WaitForEndOfFrame();
        _camera.enabled = false;
        _camera.Render();
        yield return new WaitForEndOfFrame();
        //Destroy(_enemiesSpawnTransform.gameObject);
    }

    private void SpawnEnemies()
    {
        EnemyFactory enemyFactory = EnemyFactory.GetInstance();

        Quaternion enemyRotation = Quaternion.AngleAxis(45, Vector3.up);

        for (int i = 0; i < _enemyTypesCollection.EnemyTypes.Length; ++i)
        {
            EnemyTypeConfig enemyTypeConfig = _enemyTypesCollection.EnemyTypes[i];
            if (enemyTypeConfig == _fakeEnemyType)
            {
                continue;
            }
            
            
            int row = (i / _columns);
            int column = (i % _columns);

            Vector3 position = _enemiesSpawnTransform.position + 
                               new Vector3(column * _gridSize.x, row * _gridSize.y, 0);

            GameObject enemyGameObject = 
                enemyFactory.GetEnemyGameObject(enemyTypeConfig, position, enemyRotation, _enemiesSpawnTransform);
            enemyGameObject.SetActive(true);
            Enemy enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.InitWithoutFunctionality();

            enemyTypeConfig.PhotoIndex = i;
        }
    }
}
