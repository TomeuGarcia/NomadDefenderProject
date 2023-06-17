using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMapGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("MapData where to store the generated map")] private MapData mapData;  
    [SerializeField] private OWMapGenerationSettings generationSettings;  


    public MapData GenerateMap()
    {
        GenerateNodes();
        GenerateConnections();

        return mapData;
    }


    private void GenerateNodes()
    {
        mapData.levels = new MapData.MapLevelData[generationSettings.numberOfLevels];

        // 1 width start levels
        for (int levelI = 0; levelI < generationSettings.numberOf1WidthStartLevels; ++levelI)
        {
            InstantiateNodesAtLevel(1, levelI);

            Debug.Log("srt "+1);
        }


        // other levels
        int transitionTo1WidthEnd = (int)Mathf.Max(0, generationSettings.maxWidth - 2) / generationSettings.maxWidthShrinkStep;
        int randomWidthLevelI = generationSettings.numberOfLevels - generationSettings.numberOf1WidthEndLevels - transitionTo1WidthEnd;

        int counterTotal1Width = 0;
        int counterChained1Width = 0;

        for (int levelI = generationSettings.numberOf1WidthStartLevels; levelI < randomWidthLevelI; ++levelI)
        {
            int lastLevelWidth = mapData.levels[levelI - 1].nodes.Length;
            int maxWidthThisLevel = (int)Mathf.Min(generationSettings.maxWidth, lastLevelWidth + generationSettings.maxWidthGrowStep);
            int minWidthThisLevel = (int)Mathf.Max(1, lastLevelWidth - generationSettings.maxWidthShrinkStep);
            int levelWidth = Random.Range(minWidthThisLevel, maxWidthThisLevel + 1);

            //// kinda hardcoded
            if (levelWidth == 1)
            {
                ++counterTotal1Width;
                ++counterChained1Width;

                if (counterTotal1Width > generationSettings.maxNum1Width || counterChained1Width > generationSettings.maxChained1Width)
                {
                    levelWidth = Random.Range(2, lastLevelWidth + generationSettings.maxWidthGrowStep + 1);
                }
            }
            else
            {
                counterChained1Width = 0;
            }
            ////

            InstantiateNodesAtLevel(levelWidth, levelI);

            Debug.Log("mid "+levelWidth);
        }


        // transition to 1 width
        int transitionTo1WidthEndLevelI = generationSettings.numberOfLevels - generationSettings.numberOf1WidthEndLevels;
        int levelWidthBeforeTransition = mapData.levels[randomWidthLevelI - 1].nodes.Length;
        int widthTransition = generationSettings.maxWidth - 1;

        for (int levelI = randomWidthLevelI; levelI < transitionTo1WidthEndLevelI; ++levelI)
        {
            int levelWidth = Mathf.Min(levelWidthBeforeTransition, widthTransition);
            InstantiateNodesAtLevel(levelWidth, levelI);

            Debug.Log("trn " + levelWidth);
            
            --widthTransition;
        }


        // 1 width end levels
        for (int levelI = transitionTo1WidthEndLevelI; levelI < generationSettings.numberOfLevels; ++levelI)
        {
            InstantiateNodesAtLevel(1, levelI);

            Debug.Log("end "+1);
        }

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


    private void GenerateConnections()
    {
        // TODO
        MakeAllNodeConnections();
    }

    private int GetXDistanceBetweenNodes(MapData.MapNodeData nodeA, MapData.MapNodeData nodeB)
    {
        return (int)Mathf.Abs(nodeA.xAxisPos - nodeB.xAxisPos);
    }

    private void ConnectNodes(MapData.MapNodeData currentLevelNode, MapData.MapNodeData nextLevelNode)
    {
        currentLevelNode.connectionsToNextLevel.Add(nextLevelNode);

        //int[] temp = new int[currentLevelNode.connectionsNextLevel.Length + 1];
        //currentLevelNode.connectionsNextLevel.add
    }

    private void MakeAllNodeConnections()
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
                        ConnectNodes(currentLevelNode, nextLevelNode);
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
                        // 0 = None
                        // 1 = Up_Right
                        // 2 = Up_Left
                        int randomConnection = Random.Range(0, 3);
                        if (randomConnection == 1)
                        {
                            ConnectNodes(currentLevelNodes[currentNodeI], nextLevelNodes[nextNodeI + 1]);
                        }
                        else if (randomConnection == 2)
                        {
                            ConnectNodes(currentLevelNodes[currentNodeI + 1], nextLevelNodes[nextNodeI]);
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
                    ConnectNodes(currentLevelNode, leftmostNextLevelNode);

                    ++currentNodeI;
                    leftSideXDistance = GetXDistanceBetweenNodes(currentLevelNodes[currentNodeI], leftmostNextLevelNode);
                } 
                while(leftSideXDistance >= 2);


                int rightSideXDistance = firstCurrentToFirstNextXDistance;
                currentNodeI = currentLevelNodes.Length - 1;
                MapData.MapNodeData rightmostNextLevelNode = nextLevelNodes[nextLevelNodes.Length - 1];

                do
                {
                    MapData.MapNodeData currentLevelNode = currentLevelNodes[currentNodeI];
                    ConnectNodes(currentLevelNode, leftmostNextLevelNode);

                    --currentNodeI;
                    rightSideXDistance = GetXDistanceBetweenNodes(currentLevelNodes[currentNodeI], rightmostNextLevelNode);
                }
                while (rightSideXDistance >= 2);
            }


        }
    }

}
