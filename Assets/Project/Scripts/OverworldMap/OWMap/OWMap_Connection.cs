using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class OWMap_Connection : MonoBehaviour
{
    [SerializeField] private Transform connectionTransform;

    [SerializeField] private MeshRenderer[] meshRenderers;
    private Material[] materials;


    private void Awake()
    {
        materials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            materials[i] = meshRenderers[i].material;
        }
    }

    public void InitTransform(Vector3 cNodeLocalPos, Vector3 nNodeLocalPos, Vector3 mapForwardDir)
    {
        Vector3 currentToNext = nNodeLocalPos - cNodeLocalPos;
        Vector3 dirCurrentToNext = currentToNext.normalized;

        Vector3 currentToNextMidPoint = cNodeLocalPos + (dirCurrentToNext * (currentToNext.magnitude / 2.0f));
        Quaternion connectionRotation = Quaternion.FromToRotation(mapForwardDir, dirCurrentToNext);

        connectionTransform.localPosition = currentToNextMidPoint;
        connectionTransform.localRotation = connectionRotation;
    }



    public void SetColor(UnityEngine.Color color)
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            materials[i].color = color;
        }
    }

}
