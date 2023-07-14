using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace OWmapShowcase
{

    public class Showcase_OWMapGameManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private ShowcaseHUD _showcaseHUD;

        [Header("GENERATION")]
        [SerializeField] private OWMapGenerationSettings _generationSettings;
        [SerializeField] private Showcase_OWMapGenerator _generator;        

        private bool _hasFinishedSpawningMap;

        [Header("TRANSFORMS")]
        [SerializeField] private Transform _levelsHolder;

        [Header("CONNECTION")]
        [SerializeField] private FakeConnection _connectionEvaluator;
        [SerializeField] private Material _connectionCreatedMaterial;
        [SerializeField] private Material _connectionDestroyedMaterial;
        [SerializeField] private Material _connectionSavedMaterial;

        [Header("PREFABS")]
        [SerializeField] private GameObject _levelPrefab;
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _connectionPrefab;
        private List<GameObject> _spawnedNodes;
        private Dictionary<(int, int, int), GameObject> _spawnedConnectionsMap;

        private bool _instantly = false;
        public bool Instantly => _instantly;
        

        private void Awake()
        {
            _hasFinishedSpawningMap = true;

            _nodePrefab.SetActive(false);
            _connectionPrefab.SetActive(false);

            _connectionEvaluator.SetMaterial(_connectionCreatedMaterial);
            _connectionEvaluator.gameObject.SetActive(false);

            _spawnedNodes = new List<GameObject>(_generationSettings.numberOfLevels * _generationSettings.maxWidth);
            _spawnedConnectionsMap = new Dictionary<(int, int, int), GameObject>(_generationSettings.numberOfLevels * _generationSettings.maxWidth);

            for (int i = 0; i < 15; ++i)
            {
                GameObject level = Instantiate(_levelPrefab, _levelsHolder);
                level.name = "Level " + i.ToString();
            }

            _showcaseHUD.AwakeInit(this, _generationSettings);
            _showcaseHUD.HideGenerationStatusText();

            _generator.showcaseHUD = _showcaseHUD;
        }



        private void OnEnable()
        {
            _generator.OnLevelCreated += SpawnLevel;

            _generator.OnConnectionCreationStart += ShowConnectionEvaluator;

            _generator.OnConnectionCreated += SpawnConnection;
            _generator.OnConnectionRemoved += RemoveConnection;
            _generator.OnConnectionSavedFromRemove += SaveConnection;
        }
        
        private void OnDisable()
        {
            _generator.OnLevelCreated -= SpawnLevel;

            _generator.OnConnectionCreationStart -= ShowConnectionEvaluator; 

            _generator.OnConnectionCreated -= SpawnConnection;
            _generator.OnConnectionRemoved -= RemoveConnection;
            _generator.OnConnectionSavedFromRemove -= SaveConnection;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void ResetMap()
        {
            DestroyCurrentMap();
            StartCoroutine(SpawnMap());
        }

        private IEnumerator SpawnMap()
        {
            _hasFinishedSpawningMap = false;
            if (!_instantly) _showcaseHUD.ShowGenerationStatusText();

            yield return _generator.GenerateMap(_generationSettings, _instantly);
            MapData mapData = _generator.GetMapData();

            _hasFinishedSpawningMap = true;

            _connectionEvaluator.gameObject.SetActive(false);

            _showcaseHUD.EnableStartButton();
            _showcaseHUD.HideGenerationStatusText();
        }

        private void DestroyCurrentMap()
        {
            foreach (GameObject node in _spawnedNodes)
            {
                Destroy(node);
            }
            _spawnedNodes.Clear(); 


            foreach(KeyValuePair<(int, int, int), GameObject> connection in _spawnedConnectionsMap)
            {
                Destroy(connection.Value);
            }
            _spawnedConnectionsMap.Clear();
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


        private (Vector3, Quaternion) GetConnectionPositionRotation(int fromLevelI, int fromNodeI, int toNodeI)
        {
            Vector3 fromNodePosition = _levelsHolder.GetChild(fromLevelI).GetChild(fromNodeI).position;
            Vector3 toNodePosition = _levelsHolder.GetChild(fromLevelI + 1).GetChild(toNodeI).position;
            Vector3 halfwayPosition = Vector3.LerpUnclamped(fromNodePosition, toNodePosition, 0.5f);

            Vector3 direction = (toNodePosition - fromNodePosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, transform.up);

            return (halfwayPosition, rotation);
        }

        private void SpawnConnection(int fromLevelI, int fromNodeI, int toNodeI)
        {
            (Vector3, Quaternion) positionAndRotation = GetConnectionPositionRotation(fromLevelI, fromNodeI, toNodeI);

            GameObject connection = Instantiate(_connectionPrefab, positionAndRotation.Item1, positionAndRotation.Item2, _levelsHolder.GetChild(fromLevelI));
            connection.SetActive(true);
            connection.name = "Connection " + fromNodeI.ToString() + "-" + toNodeI.ToString();

            Vector3 fromNodePosition = _levelsHolder.GetChild(fromLevelI).GetChild(fromNodeI).position;
            Vector3 toNodePosition = _levelsHolder.GetChild(fromLevelI + 1).GetChild(toNodeI).position;
            float distanceBetweenNodes = Vector3.Distance(fromNodePosition, toNodePosition);
            connection.GetComponent<FakeConnection>().SetLengthScale(distanceBetweenNodes);

            _spawnedConnectionsMap.Add((fromLevelI, fromNodeI, toNodeI), connection);

            _connectionEvaluator.transform.position = positionAndRotation.Item1;
            _connectionEvaluator.transform.rotation = positionAndRotation.Item2;


            _connectionEvaluator.SetLengthScale(distanceBetweenNodes);
        }

        private void RemoveConnection(int fromLevelI, int fromNodeI, int toNodeI)
        {
            (int, int, int) connectionKey = (fromLevelI, fromNodeI, toNodeI);

            GameObject connection = _spawnedConnectionsMap[connectionKey];

            _connectionEvaluator.SetMaterial(_connectionDestroyedMaterial);
            _connectionEvaluator.transform.position = connection.transform.position;
            _connectionEvaluator.transform.rotation = connection.transform.rotation;


            Vector3 fromNodePosition = _levelsHolder.GetChild(fromLevelI).GetChild(fromNodeI).position;
            Vector3 toNodePosition = _levelsHolder.GetChild(fromLevelI + 1).GetChild(toNodeI).position;
            _connectionEvaluator.SetLengthScale(Vector3.Distance(fromNodePosition, toNodePosition));

            _spawnedConnectionsMap.Remove(connectionKey);
            Destroy(connection);
        }
        private void SaveConnection(int fromLevelI, int fromNodeI, int toNodeI)
        {
            (int, int, int) connectionKey = (fromLevelI, fromNodeI, toNodeI);

            GameObject connection = _spawnedConnectionsMap[connectionKey];

            _connectionEvaluator.SetMaterial(_connectionSavedMaterial);
            _connectionEvaluator.transform.position = connection.transform.position;
            _connectionEvaluator.transform.rotation = connection.transform.rotation;

            Vector3 fromNodePosition = _levelsHolder.GetChild(fromLevelI).GetChild(fromNodeI).position;
            Vector3 toNodePosition = _levelsHolder.GetChild(fromLevelI + 1).GetChild(toNodeI).position;
            _connectionEvaluator.SetLengthScale(Vector3.Distance(fromNodePosition, toNodePosition));
        }




        private void ShowConnectionEvaluator()
        {
            //if (_instantly) return;
            _connectionEvaluator.gameObject.SetActive(true);
            _connectionEvaluator.SetMaterial(_connectionCreatedMaterial);
        }

        public void SetInstantly(bool instantly)
        {
            _instantly = instantly;
        }

    }

}