using System.Collections;
using System.Collections.Generic;
using UnityEngine;






[CreateAssetMenu(fileName = "OverworldMap", menuName = "Map/OverworldMap")]
public class OverworldMap : ScriptableObject
{
    [System.Serializable]
    public class MapNode
    {
        public int[] connecionsNextLevel;
    }

    [System.Serializable]
    public class MapLevel
    {
        public MapNode[] nodes;
    }


    [SerializeField] public MapLevel[] levels;




    public void PrintResult()
    {



    }


}
