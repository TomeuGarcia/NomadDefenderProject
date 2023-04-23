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
        [HideInInspector] public int xAxisPos; // Formula: nodeI*2 - (numNodesInLevel-1)       
    }

    [System.Serializable]
    public class MapLevelData
    {
        public MapNodeData[] nodes;
    }


    [SerializeField] public MapLevelData[] levels;



}
