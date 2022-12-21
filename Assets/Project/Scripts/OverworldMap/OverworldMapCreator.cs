using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverworldMapCreator : MonoBehaviour
{
    [Header("MAP DATA")]
    [SerializeField] private MapData mapData;

    [Header("PREFABS")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject nodeConnectionPrefab;
    [SerializeField] private GameObject mapTextPrefab; 
    [SerializeField] private GameObject levelHolderPrefab;

    [Header("TRANSFORMS")]
    [SerializeField] private Transform holder;


    private Vector3 mapForwardDir = Vector3.forward;
    private Vector3 mapRightDir = Vector3.right;

    private Vector3 DisplacementBetweenLevels => mapForwardDir * 2f;

    private const float NodeGapWidth = 2.0f;


    private void OnValidate()
    {
        //CleanupMap();
        //CreateMap();
    }

    private void Awake()
    {
        RegenerateMap();
    }

    public void RegenerateMap()
    {
        CleanupMap();
        CreateMap();
    }

    private void CleanupMap()
    {
        while (holder.childCount > 0)
        {
            Destroy(holder.GetChild(0));
        }
    }


    private void CreateMap()
    {
        CreateNodes();
        CreateConnections();
    }

    public void CreateNodes()
    {
        for (int levelI = 0; levelI < mapData.levels.Length; ++levelI)
        {
            MapData.MapLevel mapLevel = mapData.levels[levelI];

            Transform levelHolder = Instantiate(levelHolderPrefab, holder).transform;
            levelHolder.localPosition = DisplacementBetweenLevels * levelI;
            Transform nodesHolder = levelHolder.GetChild(0);
            Transform otherHolder = levelHolder.GetChild(2);

            for (int nodeI = 0; nodeI < mapLevel.nodes.Length; ++nodeI)
            {
                float xDisplacement = (1f - mapLevel.nodes.Length) / 2.0f;

                Transform nodeTransform = Instantiate(nodePrefab, nodesHolder).transform;
                nodeTransform.localPosition = mapRightDir * (nodeI + xDisplacement) * NodeGapWidth;
            }

            Transform textTransform = Instantiate(mapTextPrefab, otherHolder).transform;
            textTransform.GetChild(0).GetComponent<TextMeshPro>().text = levelI.ToString();
            textTransform.localPosition = mapRightDir * -3f;
        }

    }

    public void CreateConnections()
    {
        for (int levelI = 0; levelI < mapData.levels.Length - 1; ++levelI)
        {
            MapData.MapLevel mapLevel = mapData.levels[levelI];
            MapData.MapLevel mapNextLevel = mapData.levels[levelI+1];

            Transform levelHolder = holder.GetChild(levelI);
            Transform cNodesHolder = levelHolder.GetChild(0);
            Transform cConnectionsHolder = levelHolder.GetChild(1);

            Transform nextLevelHolder = holder.GetChild(levelI+1);
            Transform nNodesHolder = nextLevelHolder.GetChild(0);

            for (int nodeI = 0; nodeI < mapLevel.nodes.Length; ++nodeI)
            {
                MapData.MapNode currentNode = mapLevel.nodes[nodeI];
                Transform currentNodeTransform = cNodesHolder.GetChild(nodeI);

                int[] connectionsNextLevel = currentNode.connectionsNextLevel;
                for (int conI = 0; conI < connectionsNextLevel.Length; ++conI)
                {
                    int connectedNextNodeI = connectionsNextLevel[conI];

                    MapData.MapNode nextNode = mapNextLevel.nodes[connectedNextNodeI]; // DO whatever with this
                    Transform nextNodeTransform = nNodesHolder.GetChild(connectedNextNodeI);

                    Vector3 cPos = currentNodeTransform.localPosition;
                    Vector3 nPos = nextNodeTransform.localPosition + DisplacementBetweenLevels;

                    Vector3 currentToNext = nPos - cPos;
                    Vector3 dirCurrentToNext = currentToNext.normalized;
                    Vector3 currentToNextMidPoint = cPos + (dirCurrentToNext * (currentToNext.magnitude / 2.0f));
                    Quaternion connectionRotation = Quaternion.FromToRotation(mapForwardDir, dirCurrentToNext);
                    
                    Transform connectionTransform = Instantiate(nodeConnectionPrefab, cConnectionsHolder).transform;

                    connectionTransform.localPosition = currentToNextMidPoint;
                    connectionTransform.localRotation = connectionRotation;
                }
            }
        }

    }

    




}
