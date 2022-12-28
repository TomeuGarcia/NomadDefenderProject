using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMapGameManager : MonoBehaviour
{
    private OWMap_Node[][] mapNodes;

    [SerializeField] private OverworldMapCreator owMapCreator;


    private void Start()
    {
        owMapCreator.RegenerateMap(out mapNodes);

        // TODO decorate

        SelectFirstNode();
    }


    private void SelectFirstNode()
    {
        mapNodes[0][0].SetSelected();
    }


}
