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
    [SerializeField] private ConnectionCablePool cablePool;

    [Header("TRANSFORMS")]
    [SerializeField] private Transform holder;



    private static Vector3 MapForwardDir => Vector3.forward;
    private static Vector3 MapRightDir => Vector3.right;

    public static Vector3 DisplacementBetweenLevels => MapForwardDir * 2f;

    private const float NodeGapWidth = 2.0f;



    public void RegenerateMap(out OWMap_Node[][] mapNodes)
    {
        CleanupMap();
        CreateMap(out mapNodes);
    }

    private void CleanupMap()
    {      
        while (holder.childCount > 0)
        {
            Destroy(holder.GetChild(0));
        }
    }


    private void CreateMap(out OWMap_Node[][] mapNodes)
    {
        //// TODO move this code when precedural generation is added
        for (int levelI = 0; levelI < mapData.levels.Length; ++levelI)
        {
            int numNodesInLevel = mapData.levels[levelI].nodes.Length;
            for (int nodeI = 0; nodeI < numNodesInLevel; ++nodeI)
            {
                // Formula xAxisPos: nodeI*2 - (numNodesInLevel-1) 
                mapData.levels[levelI].nodes[nodeI].xAxisPos = (nodeI * 2) - (numNodesInLevel - 1);
            }
        }
        ////

        CreateNodes(out mapNodes);
        CreateConnections(mapNodes);
        SetNodeReferences(mapNodes);
    }

    private void CreateNodes(out OWMap_Node[][] mapNodes)
    {
        mapNodes = new OWMap_Node[mapData.levels.Length][];

        for (int levelI = 0; levelI < mapData.levels.Length; ++levelI)
        {
            MapData.MapLevelData mapLevel = mapData.levels[levelI];

            Transform levelHolder = Instantiate(levelHolderPrefab, holder).transform;
            levelHolder.localPosition = DisplacementBetweenLevels * levelI;
            Transform nodesHolder = levelHolder.GetChild(0);
            Transform otherHolder = levelHolder.GetChild(2);

            mapNodes[levelI] = new OWMap_Node[mapLevel.nodes.Length];

            for (int nodeI = 0; nodeI < mapLevel.nodes.Length; ++nodeI)
            {
                mapNodes[levelI][nodeI] = Instantiate(nodePrefab, nodesHolder).GetComponent<OWMap_Node>();
                mapNodes[levelI][nodeI].InitTransform(nodeI, mapLevel.nodes.Length, MapRightDir, NodeGapWidth);
            }

            // For easier development purposes:
            /*
            Transform textTransform = Instantiate(mapTextPrefab, otherHolder).transform;
            textTransform.GetChild(0).GetComponent<TextMeshPro>().text = levelI.ToString();
            textTransform.localPosition = MapRightDir * -3f;
            */
        }

    }

    private void CreateConnections(OWMap_Node[][] mapNodes)
    {
        int lastMapLevelIndex = mapData.levels.Length - 1;

        for (int levelI = 0; levelI < lastMapLevelIndex; ++levelI)
        {
            MapData.MapLevelData mapLevel = mapData.levels[levelI];
            MapData.MapLevelData mapNextLevel = mapData.levels[levelI+1];

            Transform levelHolder = holder.GetChild(levelI);
            Transform cNodesHolder = levelHolder.GetChild(0);
            Transform cConnectionsHolder = levelHolder.GetChild(1);

            Transform nextLevelHolder = holder.GetChild(levelI+1);
            Transform nNodesHolder = nextLevelHolder.GetChild(0);

            for (int nodeI = 0; nodeI < mapLevel.nodes.Length; ++nodeI) //node iteration
            {
                MapData.MapNodeData currentNode = mapLevel.nodes[nodeI];
                Transform currentNodeTransform = cNodesHolder.GetChild(nodeI);

                List<OWMap_Connection> nextLevelConnections = new List<OWMap_Connection>(); // Connections Reference

                int[] connectionsNextLevel = currentNode.connectionsNextLevel;
                for (int conI = 0; conI < connectionsNextLevel.Length; ++conI) //connection iteration
                {
                    int connectedNextNodeI = connectionsNextLevel[conI];

                    MapData.MapNodeData nextNode = mapNextLevel.nodes[connectedNextNodeI]; // DO whatever with this
                    Transform nextNodeTransform = nNodesHolder.GetChild(connectedNextNodeI);

                    Vector3 cPos = currentNodeTransform.localPosition;
                    Vector3 nPos = nextNodeTransform.localPosition + DisplacementBetweenLevels;

                    OWMap_Connection owMapConnection = Instantiate(nodeConnectionPrefab, cConnectionsHolder).GetComponent<OWMap_Connection>();
                    owMapConnection.InitTransform(cPos, nPos, MapForwardDir);

                    int connectionType = currentNode.xAxisPos - nextNode.xAxisPos;
                    GameObject calbe = cablePool.GetConnectionCable(Mathf.Abs(connectionType));
                    owMapConnection.SetConnection(calbe, (connectionType > 0));

                    nextLevelConnections.Add(owMapConnection); // Connections Reference
                }

                // Set Connections Reference
                mapNodes[levelI][nodeI].SetNextLevelConnections(nextLevelConnections.ToArray());
            }
        }

    }


    private void SetNodeReferences(OWMap_Node[][] mapNodes)
    {
        int lastMapLevelIndex = mapData.levels.Length - 1;

        for (int levelI = 0; levelI < lastMapLevelIndex; ++levelI)
        {
            MapData.MapLevelData mapLevel = mapData.levels[levelI];

            for (int nodeI = 0; nodeI < mapLevel.nodes.Length; ++nodeI)
            {
                MapData.MapNodeData currentNode = mapLevel.nodes[nodeI];

                List<OWMap_Node> nextLevelNodes = new List<OWMap_Node>(); // NodeReferences

                int[] connectionsNextLevel = currentNode.connectionsNextLevel;
                for (int conI = 0; conI < connectionsNextLevel.Length; ++conI)
                {
                    int connectedNextNodeI = connectionsNextLevel[conI];

                    nextLevelNodes.Add(mapNodes[levelI + 1][connectedNextNodeI]); // NodeReferences
                }

                // NodeReferences
                List<OWMap_Node> neighbourLevelNodes = new List<OWMap_Node>();
                for (int nodeJ = 0; nodeJ < mapLevel.nodes.Length; ++nodeJ)
                {
                    if (nodeJ != nodeI)
                        neighbourLevelNodes.Add(mapNodes[levelI][nodeJ]);
                }

                // Set NodeReferences
                OWMap_Node.MapReferencesData mapReferencesData =
                    new OWMap_Node.MapReferencesData(levelI, nodeI,
                                                     nextLevelNodes.ToArray(), neighbourLevelNodes.ToArray());
                mapNodes[levelI][nodeI].InitMapReferencesData(mapReferencesData);
            }
        }

        // Set NodeReferences
        for (int nodeI = 0; nodeI < mapNodes[lastMapLevelIndex].Length; ++nodeI)
        {
            OWMap_Node.MapReferencesData lastNode_mapReferencesData =
            new OWMap_Node.MapReferencesData(lastMapLevelIndex, 0, true);
            mapNodes[lastMapLevelIndex][nodeI].InitMapReferencesData(lastNode_mapReferencesData);
        }
        
    }


}
