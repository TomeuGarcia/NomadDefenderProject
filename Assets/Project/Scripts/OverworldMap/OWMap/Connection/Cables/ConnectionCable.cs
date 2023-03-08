using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionCable : MonoBehaviour
{
    [SerializeField] protected int cableAmount;
    [SerializeField] protected List<SkinnedMeshRenderer> calbeMeshes = new List<SkinnedMeshRenderer>();
    protected List<Material> cableMaterials = new List<Material>();

    [SerializeField] protected MaterialLerp.FloatData lerpData;

    private void Awake()
    {
        foreach(SkinnedMeshRenderer mesh in calbeMeshes)
        {
            foreach (Material material in mesh.materials)
            {
                cableMaterials.Add(material);
            }
        }
    }

    public virtual void FillCable(bool destroyed)
    {
        if (destroyed) { foreach (Material mat in cableMaterials) { mat.SetFloat("_Broken", 1.0f); mat.SetFloat("_ConnectionCoef", 0.0f); } }
    }

    public virtual void HoverCable() { }
}
