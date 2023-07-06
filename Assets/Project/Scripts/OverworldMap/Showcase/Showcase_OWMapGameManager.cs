using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OWmapShowcase
{

    public class Showcase_OWMapGameManager : MonoBehaviour
    {
        [SerializeField] private OWMapGenerationSettings _generationSettings;
        [SerializeField] private Showcase_OWMapGenerator _generator;

        private bool _hasFinishedSpawningMap;

        [SerializeField] private Transform _levelsHolder;
        [SerializeField] private GameObject _connectionCreated;
        [SerializeField] private GameObject _connectionDestroyed;        

        [SerializeField] private GameObject _levelPrefab;
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _connectionPrefab;
        private List<GameObject> _spawnedNodes;
        private List<GameObject> _spawnedConnections;


        private void Awake()
        {
            _hasFinishedSpawningMap = true;

            _nodePrefab.SetActive(false);
            _connectionPrefab.SetActive(false);
            _connectionCreated.SetActive(false);
            _connectionDestroyed.SetActive(false);

            _spawnedNodes = new List<GameObject>(_generationSettings.numberOfLevels * _generationSettings.maxWidth);
            _spawnedConnections = new List<GameObject>(_generationSettings.numberOfLevels * _generationSettings.maxWidth);

            for (int i = 0; i < _generationSettings.numberOfLevels; ++i)
            {
                GameObject level = Instantiate(_levelPrefab, _levelsHolder);
                level.name = "Level " + i.ToString();
            }

        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && _hasFinishedSpawningMap)
            {
                DestroyCurrentMap();
                StartCoroutine(SpawnMap());
            }
        }


        private void OnEnable()
        {
            _generator.OnLevelCreated += SpawnLevel;
            _generator.OnConnectionCreated += SpawnConnection;
        }
        
        private void OnDisable()
        {
            _generator.OnLevelCreated -= SpawnLevel;
            _generator.OnConnectionCreated -= SpawnConnection;
        }


        private IEnumerator SpawnMap()
        {
            _hasFinishedSpawningMap = false;

            yield return _generator.GenerateMap();
            MapData mapData = _generator.GetMapData();

            _hasFinishedSpawningMap = true;

            _connectionCreated.SetActive(false);
        }

        private void DestroyCurrentMap()
        {
            foreach (GameObject node in _spawnedNodes)
            {
                Destroy(node);
            }
            _spawnedNodes.Clear(); 

            foreach (GameObject connection in _spawnedConnections)
            {
                Destroy(connection);
            }
            _spawnedConnections.Clear();

        }

        private void SpawnLevel(int levelI, MapData.MapNodeData[] level)
        {
            Vector3 levelCenterPosition = transform.position + (transform.forward * levelI * 2.0f);

            Vector3 nodePositionOffset = transform.right * 1.5f;

            float divisions = level.Length - 1.0f;
            Vector3 firstNodeOffset = nodePositionOffset * (-divisions / 2.0f);

            for (int nodeI = 0; nodeI < level.Length; ++nodeI)
            {
                Vector3 nodePosition = levelCenterPosition + firstNodeOffset + (nodePositionOffset * nodeI);

                GameObject node = Instantiate(_nodePrefab, nodePosition, Quaternion.identity, _levelsHolder.GetChild(levelI));
                node.SetActive(true);
                node.name = "Node " + nodeI.ToString();

                _spawnedNodes.Add(node);
            }
        }

        private void SpawnConnection(int fromLevelI, int fromNodeI, int toNodeI)
        {
            Vector3 fromNodePosition = _levelsHolder.GetChild(fromLevelI).GetChild(fromNodeI).position;
            Vector3 toNodePosition = _levelsHolder.GetChild(fromLevelI+1).GetChild(toNodeI).position;
            Vector3 halfwayPosition = Vector3.LerpUnclamped(fromNodePosition, toNodePosition, 0.5f);

            Vector3 direction = (toNodePosition - fromNodePosition).normalized;
            Quaternion rotation =Quaternion.LookRotation(direction, transform.up);

            GameObject connection = Instantiate(_connectionPrefab, halfwayPosition, rotation, _levelsHolder.GetChild(fromLevelI));
            connection.SetActive(true);
            connection.name = "Connection " + fromNodeI.ToString() + "-" + toNodeI.ToString();

            _spawnedConnections.Add(connection);

            _connectionCreated.SetActive(true);
            _connectionCreated.transform.position = halfwayPosition;
            _connectionCreated.transform.rotation = rotation;
        }



    }

}