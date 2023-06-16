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
    }

}
