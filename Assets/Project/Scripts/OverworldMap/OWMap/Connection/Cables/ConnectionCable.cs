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

    public virtual void FillCable(bool destroyed) { }
}
