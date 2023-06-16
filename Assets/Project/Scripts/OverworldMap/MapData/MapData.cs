using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewMapData", menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    [System.Serializable]
    public class MapNodeData
    {
        public int[] connectionsNextLevel;
        [HideInInspector] public int xAxisPos;
        
        public MapNodeData()
        {
        }
        public MapNodeData(int nodeI, int numNodesInLevel)
        {
            SetXAxisPos(nodeI, numNodesInLevel);
        }

        public void SetXAxisPos(int nodeI, int numNodesInLevel)
        {
            // Formula: nodeI*2 - (numNodesInLevel-1)       
            xAxisPos = (nodeI * 2) - (numNodesInLevel - 1);
        }
    }

    [System.Serializable]
    public class MapLevelData
    {
        public MapNodeData[] nodes;

        public MapLevelData(MapNodeData[] nodes)
        {
            this.nodes = nodes;
        }
    }


    [SerializeField] public MapLevelData[] levels;



}
