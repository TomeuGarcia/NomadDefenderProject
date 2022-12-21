using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMapCreator : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject nodeConnectionPrefab;

    [SerializeField] private OverworldMap overworldMap;

    [SerializeField] private Transform holder;


    public void CleanupMap()
    {
        while (holder.childCount > 0)
        {
            Destroy(holder.GetChild(0));
        }
    }


    public void CreateMap()
    {

        for (int levelI = 0; levelI < overworldMap.levels.Length; ++levelI)
        {
            OverworldMap.MapLevel mapLevel = overworldMap.levels[levelI];
            Instantiate(new GameObject(), new Vector3(0, 0, levelI), Quaternion.identity, holder);

            for (int nodeI = 0; nodeI < mapLevel.nodes.Length; ++nodeI)
            {
                float displacement = (1 - mapLevel.nodes.Length) / 2.0f;
                Instantiate(nodePrefab, new Vector3(nodeI + displacement, 0, 0), Quaternion.identity, holder.GetChild(levelI));
            }
        }    


    }






}
