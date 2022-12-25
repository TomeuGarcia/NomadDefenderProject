using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewMapData", menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    [System.Serializable]
    public class MapNode
    {
        public int[] connectionsNextLevel;
    }

    [System.Serializable]
    public class MapLevel
    {
        public MapNode[] nodes;
    }


    [SerializeField] public MapLevel[] levels;



}
