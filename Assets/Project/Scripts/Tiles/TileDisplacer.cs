using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDisplacer : MonoBehaviour
{
    [SerializeField] private Transform groundTilesParent;

    private Transform[] groundTilesTransforms;


    private void Awake()
    {
        groundTilesTransforms = new Transform[groundTilesParent.childCount];
        for (int i = 0; i< groundTilesTransforms.Length; i++)
        {
            groundTilesTransforms[i] = groundTilesParent.GetChild(i);
        }

        DisplaceGroundTiles();
    }

    private void DisplaceGroundTiles()
    {
        foreach (Transform tileTransform in groundTilesTransforms)
        {
            tileTransform.position += transform.up * Random.Range(0.05f, 0.1f);
        }
    }

}
