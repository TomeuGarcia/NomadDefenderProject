using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OWmapShowcase
{
    public class Showcase_OWMapGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("MapData where to store the generated map")] private MapData mapData;
        private OWMapGenerationSettings _generationSettings;

        [SerializeField] private float DELAY_LEVEL_SPAWN = 0.3f;
        [SerializeField] private float DELAY_CONNECTION_SPAWN = 0.4f;
        [SerializeField] private float DELAY_CONNECTION_DELETE = 0.4f;


        // int --> levelI
        // MapData.MapNodeData[] --> level
        public Action<int, MapData.MapNodeData[]> OnLevelCreated;

        // int --> fromLevelI
        // int --> fromNodeI
        // int --> toNodeI
        public Action<int, int, int> OnConnectionCreated;
        public Action<int, int, int> OnConnectionRemoved;
        public Action<int, int, int> OnConnectionSavedFromRemove;

        public Action OnConnectionCreationStart;



        public IEnumerator GenerateMap(OWMapGenerationSettings generationSettings)
        {
            _generationSettings = generationSettings;
            yield return StartCoroutine(GenerateNodes());
            yield return StartCoroutine(GenerateConnections());            
        }
        public MapData GetMapData()
        {
            return mapData;
        }


        private IEnumerator GenerateNodes()
        {
            mapData.levels = new MapData.MapLevelData[_generationSettings.numberOfLevels];

            yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);

            // 1 width start levels
            for (int levelI = 0; levelI < _generationSettings.numberOf1WidthStartLevels; ++levelI)
            {
                InstantiateNodesAtLevel(1, levelI);

                //Debug.Log("srt " + 1);

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes);
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }


            // other levels
            int transitionTo1WidthEnd = (int)Mathf.Max(0, _generationSettings.maxWidth - 2) / _generationSettings.maxWidthShrinkStep;
            int randomWidthLevelI = _generationSettings.numberOfLevels - _generationSettings.numberOf1WidthEndLevels - transitionTo1WidthEnd;

            _counterTotal1Width = 0;
            _counterChained1Width = 0;
            _sameWidthRepeatCount = 0;

            for (int levelI = _generationSettings.numberOf1WidthStartLevels; levelI < randomWidthLevelI; ++levelI)
            {
                GenerateMiddleLevel(levelI, _generationSettings.maxWidth);

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes); 
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }


            // transition to 1 width
            int transitionTo1WidthEndLevelI = _generationSettings.numberOfLevels - _generationSettings.numberOf1WidthEndLevels;
            int widthTransition = _generationSettings.maxWidth - 1;

            for (int levelI = randomWidthLevelI; levelI < transitionTo1WidthEndLevelI; ++levelI)
            {
                GenerateMiddleLevel(levelI, widthTransition);

                --widthTransition;

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes); 
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }


            // 1 width end levels
            for (int levelI = transitionTo1WidthEndLevelI; levelI < _generationSettings.numberOfLevels; ++levelI)
            {
                InstantiateNodesAtLevel(1, levelI);

                //Debug.Log("end " + 1);

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes); 
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }

        }

        private int _counterTotal1Width;
        private int _counterChained1Width;
        private int _sameWidthRepeatCount;
        private void GenerateMiddleLevel(int levelI, int maxWidth)
        {
            int lastLevelWidth = mapData.levels[levelI - 1].nodes.Length;
            int maxWidthThisLevel = (int)Mathf.Min(maxWidth, lastLevelWidth + _generationSettings.maxWidthGrowStep);
            int minWidthThisLevel = (int)Mathf.Max(_generationSettings.minWidth, lastLevelWidth - _generationSettings.maxWidthShrinkStep);
            int levelWidth = UnityEngine.Random.Range(minWidthThisLevel, maxWidthThisLevel + 1);


            //// kinda hardcoded
            if (levelWidth == 1)
            {
                ++_counterTotal1Width;
                ++_counterChained1Width;

                if (_counterTotal1Width > _generationSettings.maxNum1Width || _counterChained1Width > _generationSettings.maxChained1Width)
                {
                    minWidthThisLevel = 2;
                    levelWidth = UnityEngine.Random.Range(minWidthThisLevel, maxWidthThisLevel);
                }
            }
            else
            {
                _counterChained1Width = 0;
            }
            ////


            _sameWidthRepeatCount = (levelWidth == lastLevelWidth) ? (_sameWidthRepeatCount + 1) : 0;
            if (_sameWidthRepeatCount >= _generationSettings.maxSameWidthRepeatTimes)
            {
                if (minWidthThisLevel == maxWidthThisLevel)
                {
                    levelWidth = minWidthThisLevel;
                }
                else
                {
                    do
                    {
                        levelWidth = UnityEngine.Random.Range(minWidthThisLevel, maxWidthThisLevel + 1);
                    }
                    while (levelWidth == lastLevelWidth);
                }

                _sameWidthRepeatCount = 0;
            }


            InstantiateNodesAtLevel(levelWidth, levelI);

            //Debug.Log("mid " + levelWidth);
        }


        private void InstantiateNodesAtLevel(int numberOfNodes, int levelI)
        {
            MapData.MapNodeData[] nodes = new MapData.MapNodeData[numberOfNodes];
            mapData.levels[levelI] = new MapData.MapLevelData(nodes);

            for (int nodeI = 0; nodeI < numberOfNodes; ++nodeI)
            {
                mapData.levels[levelI].nodes[nodeI] = new MapData.MapNodeData(nodeI, numberOfNodes);
            }
        }


        private IEnumerator GenerateConnections()
        {
            OnConnectionCreationStart?.Invoke();
            yield return StartCoroutine(MakeAllNodeConnections());
            yield return StartCoroutine(RemoveNodeConnectionsRandomly());
        }

        private int GetXDistanceBetweenNodes(MapData.MapNodeData nodeA, MapData.MapNodeData nodeB)
        {
            return (int)Mathf.Abs(nodeA.xAxisPos - nodeB.xAxisPos);
        }

        private void ConnectNodes(MapData.MapNodeData currentLevelNode, MapData.MapNodeData nextLevelNode, int fromLevelI, int fromNodeI, int toNodeI)
        {
            currentLevelNode.connectionsToNextLevel.Add(nextLevelNode);
            nextLevelNode.connectionsFromPreviousLevel.Add(currentLevelNode);

            currentLevelNode.connectionsNextLevel.Add(nextLevelNode.nodeI);

            OnConnectionCreated?.Invoke(fromLevelI, fromNodeI, toNodeI);
        }

        private void DisconnectNodes(MapData.MapNodeData currentLevelNode, MapData.MapNodeData nextLevelNode, int fromLevelI, int fromNodeI)
        {
            currentLevelNode.connectionsToNextLevel.Remove(nextLevelNode);
            nextLevelNode.connectionsFromPreviousLevel.Remove(currentLevelNode);

            currentLevelNode.connectionsNextLevel.Remove(nextLevelNode.nodeI);

            OnConnectionRemoved?.Invoke(fromLevelI, fromNodeI, nextLevelNode.nodeI);
        }

        private IEnumerator MakeAllNodeConnections()
        {
            for (int levelI = 0; levelI < mapData.levels.Length - 1; ++levelI)
            {
                MapData.MapNodeData[] currentLevelNodes = mapData.levels[levelI].nodes;
                MapData.MapNodeData[] nextLevelNodes = mapData.levels[levelI + 1].nodes;

                // 0-1-distance connections
                for (int currentNodeI = 0; currentNodeI < currentLevelNodes.Length; ++currentNodeI)
                {
                    MapData.MapNodeData currentLevelNode = currentLevelNodes[currentNodeI];

                    for (int nextNodeI = 0; nextNodeI < nextLevelNodes.Length; ++nextNodeI)
                    {
                        MapData.MapNodeData nextLevelNode = nextLevelNodes[nextNodeI];

                        int xDistance = GetXDistanceBetweenNodes(currentLevelNode, nextLevelNode);
                        if (xDistance <= 1)
                        {
                            ConnectNodes(currentLevelNode, nextLevelNode, levelI, currentNodeI, nextNodeI);
                            yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
                        }
                    }
                }

                
                bool bothLevelsHaveMoreThan1Node = currentLevelNodes.Length > 1 && nextLevelNodes.Length > 1;
                if (bothLevelsHaveMoreThan1Node)
                {
                    // 2-distance crossed connections
                    bool isCurrentLevelPair = currentLevelNodes.Length % 2 == 0;
                    bool isNextLevelPair = nextLevelNodes.Length % 2 == 0;

                    if (!(isCurrentLevelPair ^ isNextLevelPair)) // when both levels are odd or pair
                    {
                        int currentNodeI = 0;
                        int nextNodeI = 0;

                        if (currentLevelNodes.Length > nextLevelNodes.Length)
                        {
                            do
                            {
                                ++currentNodeI;
                            }
                            while (GetXDistanceBetweenNodes(currentLevelNodes[currentNodeI], nextLevelNodes[nextNodeI]) > 0);
                        }
                        else if (currentLevelNodes.Length < nextLevelNodes.Length)
                        {
                            do
                            {
                                ++nextNodeI;
                            }
                            while (GetXDistanceBetweenNodes(currentLevelNodes[currentNodeI], nextLevelNodes[nextNodeI]) > 0);
                        }

                        do
                        {
                            float randomValue = UnityEngine.Random.Range(0f, 1f);

                            if (randomValue > _generationSettings.diagonalNoConnectionThreshold &&
                                randomValue < _generationSettings.diagonalUpRightConnectionThreshold)
                            {
                                ConnectNodes(currentLevelNodes[currentNodeI], nextLevelNodes[nextNodeI + 1], levelI, currentNodeI, nextNodeI + 1);
                                yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
                            }
                            else if (randomValue > _generationSettings.diagonalUpRightConnectionThreshold &&
                                     randomValue < _generationSettings.diagonalUpLeftConnectionThreshold)
                            {
                                ConnectNodes(currentLevelNodes[currentNodeI + 1], nextLevelNodes[nextNodeI], levelI, currentNodeI + 1, nextNodeI);
                                yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
                            }

                            ++currentNodeI;
                            ++nextNodeI;
                        }
                        while (currentNodeI < currentLevelNodes.Length - 1 && nextNodeI < nextLevelNodes.Length - 1);
                    }
                }


                // 2-N edge connections
                int firstCurrentToFirstNextXDistance = GetXDistanceBetweenNodes(currentLevelNodes[0], nextLevelNodes[0]);
                bool edgeConnectionsExist = firstCurrentToFirstNextXDistance >= 2;
                if (edgeConnectionsExist)
                {
                    bool currentLevelIsWider = currentLevelNodes.Length > nextLevelNodes.Length;

                    if (currentLevelIsWider)
                    {
                        yield return StartCoroutine(CreateEdgeConnections(currentLevelNodes, nextLevelNodes, levelI, currentLevelIsWider));
                    }
                    else
                    {
                        yield return StartCoroutine(CreateEdgeConnections(nextLevelNodes, currentLevelNodes, levelI, currentLevelIsWider));
                    }
                }

            }
        }

        
        private IEnumerator CreateEdgeConnections(MapData.MapNodeData[] widerLevelNodes, MapData.MapNodeData[] shorterLevelNodes, int levelI, bool widerIsCorrentLevel)
        {
            int leftSideXDistance = 9999;

            MapData.MapNodeData leftmostShorterLevelNode = shorterLevelNodes[0];
            int widerNodeI = 0;
            MapData.MapNodeData leftSideWiderLevelNode = widerLevelNodes[widerNodeI];


            do
            {
                if (widerIsCorrentLevel)
                {
                    ConnectNodes(leftSideWiderLevelNode, leftmostShorterLevelNode, levelI, widerNodeI, 0);
                }
                else
                {
                    ConnectNodes(leftmostShorterLevelNode, leftSideWiderLevelNode, levelI, 0, widerNodeI);
                }

                ++widerNodeI;
                leftSideWiderLevelNode = widerLevelNodes[widerNodeI];

                leftSideXDistance = GetXDistanceBetweenNodes(leftSideWiderLevelNode, leftmostShorterLevelNode);

                yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
            }
            while (leftSideXDistance >= 2);


            int rightSideXDistance = 9999;
            MapData.MapNodeData rightmostShorterLevelNode = shorterLevelNodes[shorterLevelNodes.Length - 1];
            widerNodeI = widerLevelNodes.Length - 1;
            MapData.MapNodeData rightSideWiderLevelNode = widerLevelNodes[widerNodeI];

            do
            {
                if (widerIsCorrentLevel)
                {
                    ConnectNodes(rightSideWiderLevelNode, rightmostShorterLevelNode, levelI, widerNodeI, shorterLevelNodes.Length - 1);
                }
                else
                {
                    ConnectNodes(rightmostShorterLevelNode, rightSideWiderLevelNode, levelI, shorterLevelNodes.Length - 1, widerNodeI);
                }

                --widerNodeI;
                rightSideWiderLevelNode = widerLevelNodes[widerNodeI];

                rightSideXDistance = GetXDistanceBetweenNodes(rightSideWiderLevelNode, rightmostShorterLevelNode);

                yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
            }
            while (rightSideXDistance >= 2);
        }
        


        private IEnumerator RemoveNodeConnectionsRandomly()
        {
            for (int levelI = 0; levelI < mapData.levels.Length - 1; ++levelI)
            {
                MapData.MapNodeData[] currentLevelNodes = mapData.levels[levelI].nodes;

                for (int currentNodeI = 0; currentNodeI < currentLevelNodes.Length; ++currentNodeI)
                {
                    List<MapData.MapNodeData> connectionsToNextLevel = currentLevelNodes[currentNodeI].connectionsToNextLevel;


                    // Remove connections exceeding max limit
                    int numExceedingConnections = connectionsToNextLevel.Count - _generationSettings.maxConnectionsPerNode;
                    int itCount = 0;
                    int maxItCount = 100;
                    while (numExceedingConnections > 0 && itCount < maxItCount)
                    {
                        int connectionI = UnityEngine.Random.Range(0, connectionsToNextLevel.Count);

                        if (connectionsToNextLevel[connectionI].connectionsFromPreviousLevel.Count > 1)
                        {
                            DisconnectNodes(currentLevelNodes[currentNodeI], connectionsToNextLevel[connectionI], levelI, currentNodeI);
                            --numExceedingConnections;
                            
                            yield return new WaitForSeconds(DELAY_CONNECTION_DELETE);
                        }

                        ++itCount;
                    }
                    if (itCount == maxItCount)
                    {
                        Debug.Log("ERROR! maxConnectionsPerNode parameter is TOO SMALL");
                    }


                    // Remove connections randomly
                    for (int toNextI = 0; toNextI < connectionsToNextLevel.Count && connectionsToNextLevel.Count > 1; ++toNextI)
                    {
                        if (connectionsToNextLevel[toNextI].connectionsFromPreviousLevel.Count > 1)
                        {
                            float randomValue = UnityEngine.Random.Range(0f, 1f);

                            if (randomValue < _generationSettings.removeConnectionThreshold)
                            {
                                DisconnectNodes(currentLevelNodes[currentNodeI], connectionsToNextLevel[toNextI], levelI, currentNodeI);
                                --toNextI;
                            
                                yield return new WaitForSeconds(DELAY_CONNECTION_DELETE);
                            }
                            else
                            {
                                OnConnectionSavedFromRemove?.Invoke(levelI, currentNodeI, connectionsToNextLevel[toNextI].nodeI);
                                yield return new WaitForSeconds(DELAY_CONNECTION_DELETE);
                            }
                        }
                    }
                }

            }

        }
    }

}

