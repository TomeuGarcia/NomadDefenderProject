using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapData;

public class OWMap_Node : MonoBehaviour
{
    [SerializeField] private Transform nodeTransform;

    public void InitTransform(int nodeI, int numLevelNodes, Vector3 mapRightDir, float nodeGapWidth)
    {
        float centerDisplacement = (1f - numLevelNodes) / 2.0f;
        nodeTransform.localPosition = mapRightDir * (nodeI + centerDisplacement) * nodeGapWidth;
    }

}
