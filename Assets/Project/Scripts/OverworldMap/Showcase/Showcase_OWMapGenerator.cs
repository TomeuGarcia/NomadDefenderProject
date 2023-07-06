using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OWmapShowcase
{
    public class Showcase_OWMapGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("MapData where to store the generated map")] private MapData mapData;
        [SerializeField] private OWMapGenerationSettings generationSettings;

        private const float DELAY_LEVEL_SPAWN = 0.2f;
        private const float DELAY_CONNECTION_SPAWN = 0.2f;


        // int --> levelI
        // MapData.MapNodeData[] --> level
        public Action<int, MapData.MapNodeData[]> OnLevelCreated;

        // int --> fromLevelI
        // int --> fromNodeI
        // int --> toNodeI
        public Action<int, int, int> OnConnectionCreated;



        public IEnumerator GenerateMap()
        {
            yield return StartCoroutine(GenerateNodes());
            yield return StartCoroutine(GenerateConnections());            
        }
        public MapData GetMapData()
        {
            return mapData;
        }


        private IEnumerator GenerateNodes()
        {
            mapData.levels = new MapData.MapLevelData[generationSettings.numberOfLevels];

            yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);

            // 1 width start levels
            for (int levelI = 0; levelI < generationSettings.numberOf1WidthStartLevels; ++levelI)
            {
                InstantiateNodesAtLevel(1, levelI);

                //Debug.Log("srt " + 1);

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes);
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }


            // other levels
            int transitionTo1WidthEnd = (int)Mathf.Max(0, generationSettings.maxWidth - 2) / generationSettings.maxWidthShrinkStep;
            int randomWidthLevelI = generationSettings.numberOfLevels - generationSettings.numberOf1WidthEndLevels - transitionTo1WidthEnd;

            _counterTotal1Width = 0;
            _counterChained1Width = 0;
            _sameWidthRepeatCount = 0;

            for (int levelI = generationSettings.numberOf1WidthStartLevels; levelI < randomWidthLevelI; ++levelI)
            {
                GenerateMiddleLevel(levelI, generationSettings.maxWidth);

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes); 
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }


            // transition to 1 width
            int transitionTo1WidthEndLevelI = generationSettings.numberOfLevels - generationSettings.numberOf1WidthEndLevels;
            int widthTransition = generationSettings.maxWidth - 1;

            for (int levelI = randomWidthLevelI; levelI < transitionTo1WidthEndLevelI; ++levelI)
            {
                GenerateMiddleLevel(levelI, widthTransition);

                --widthTransition;

                OnLevelCreated?.Invoke(levelI, mapData.levels[levelI].nodes); 
                yield return new WaitForSeconds(DELAY_LEVEL_SPAWN);
            }


            // 1 width end levels
            for (int levelI = transitionTo1WidthEndLevelI; levelI < generationSettings.numberOfLevels; ++levelI)
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
            int maxWidthThisLevel = (int)Mathf.Min(maxWidth, lastLevelWidth + generationSettings.maxWidthGrowStep);
            int minWidthThisLevel = (int)Mathf.Max(generationSettings.minWidth, lastLevelWidth - generationSettings.maxWidthShrinkStep);
            int levelWidth = UnityEngine.Random.Range(minWidthThisLevel, maxWidthThisLevel + 1);


            //// kinda hardcoded
            if (levelWidth == 1)
            {
                ++_counterTotal1Width;
                ++_counterChained1Width;

                if (_counterTotal1Width > generationSettings.maxNum1Width || _counterChained1Width > generationSettings.maxChained1Width)
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
            if (_sameWidthRepeatCount >= generationSettings.maxSameWidthRepeatTimes)
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
            yield return StartCoroutine(MakeAllNodeConnections());
            RemoveNodeConnectionsRandomly();
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

        private void DisconnectNodes(MapData.MapNodeData currentLevelNode, MapData.MapNodeData nextLevelNode)
        {
            currentLevelNode.connectionsToNextLevel.Remove(nextLevelNode);
            nextLevelNode.connectionsFromPreviousLevel.Remove(currentLevelNode);

            currentLevelNode.connectionsNextLevel.Remove(nextLevelNode.nodeI);
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

                            if (xDistance == 1)
                            {
                                Debug.Log(levelI + ": " + currentNodeI + "-" + nextNodeI);
                            }
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

                            if (randomValue > generationSettings.diagonalNoConnectionThreshold &&
                                randomValue < generationSettings.diagonalUpRightConnectionThreshold)
                            {
                                ConnectNodes(currentLevelNodes[currentNodeI], nextLevelNodes[nextNodeI + 1], levelI, currentNodeI, nextNodeI);
                                yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
                            }
                            else if (randomValue > generationSettings.diagonalUpRightConnectionThreshold &&
                                     randomValue < generationSettings.diagonalUpLeftConnectionThreshold)
                            {
                                ConnectNodes(currentLevelNodes[currentNodeI + 1], nextLevelNodes[nextNodeI], levelI, currentNodeI, nextNodeI);
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
                    int leftSideXDistance = firstCurrentToFirstNextXDistance;
                    int currentNodeI = 0;
                    MapData.MapNodeData leftmostNextLevelNode = nextLevelNodes[0];

                    do
                    {
                        MapData.MapNodeData currentLevelNode = currentLevelNodes[currentNodeI];
                        ConnectNodes(currentLevelNode, leftmostNextLevelNode, levelI, currentNodeI, 0);

                        ++currentNodeI;
                        leftSideXDistance = GetXDistanceBetweenNodes(currentLevelNodes[currentNodeI], leftmostNextLevelNode);

                        yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
                    }
                    while (leftSideXDistance >= 2);


                    int rightSideXDistance = firstCurrentToFirstNextXDistance;
                    currentNodeI = currentLevelNodes.Length - 1;
                    MapData.MapNodeData rightmostNextLevelNode = nextLevelNodes[nextLevelNodes.Length - 1];
                    
                    do
                    {
                        MapData.MapNodeData currentLevelNode = currentLevelNodes[currentNodeI];
                        ConnectNodes(currentLevelNode, leftmostNextLevelNode, levelI, currentNodeI, 0);

                        --currentNodeI;
                        rightSideXDistance = GetXDistanceBetweenNodes(currentLevelNodes[currentNodeI], rightmostNextLevelNode);

                        yield return new WaitForSeconds(DELAY_CONNECTION_SPAWN);
                    }
                    while (rightSideXDistance >= 2);
                }


            }
        }



        private void RemoveNodeConnectionsRandomly()
        {
            for (int levelI = 0; levelI < mapData.levels.Length - 1; ++levelI)
            {
                MapData.MapNodeData[] currentLevelNodes = mapData.levels[levelI].nodes;

                for (int currentNodeI = 0; currentNodeI < currentLevelNodes.Length; ++currentNodeI)
                {
                    List<MapData.MapNodeData> connectionsToNextLevel = currentLevelNodes[currentNodeI].connectionsToNextLevel;


                    // Remove connections exceeding max limit
                    int numExceedingConnections = connectionsToNextLevel.Count - generationSettings.maxConnectionsPerNode;
                    while (numExceedingConnections > 0)
                    {
                        int connectionI = UnityEngine.Random.Range(0, connectionsToNextLevel.Count);

                        if (connectionsToNextLevel[connectionI].connectionsFromPreviousLevel.Count > 1)
                        {
                            DisconnectNodes(currentLevelNodes[currentNodeI], connectionsToNextLevel[connectionI]);
                            --numExceedingConnections;
                        }
                    }


                    // Remove connections randomly
                    for (int toNextI = 0; toNextI < connectionsToNextLevel.Count && connectionsToNextLevel.Count > 1; ++toNextI)
                    {
                        if (connectionsToNextLevel[toNextI].connectionsFromPreviousLevel.Count > 1)
                        {
                            float randomValue = UnityEngine.Random.Range(0f, 1f);

                            if (randomValue < generationSettings.removeConnectionThreshold)
                            {
                                DisconnectNodes(currentLevelNodes[currentNodeI], connectionsToNextLevel[toNextI]);
                                --toNextI;
                            }
                        }
                    }
                }

            }

        }
    }

}

