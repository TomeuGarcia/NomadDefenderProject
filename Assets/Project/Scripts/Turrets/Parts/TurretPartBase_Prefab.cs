using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretPartBase_Prefab : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [HideInInspector] private List<Material> defaultMaterials = new List<Material>();
    [SerializeField] private Material previewMaterial;
    private Material[] previewMaterials;

    private void Awake()
    {
        previewMaterials = new Material[meshRenderer.materials.Length];

        for (int i = 0; i < meshRenderer.materials.Length; ++i)
        {
            defaultMaterials.Add(meshRenderer.materials[i]);
            previewMaterials[i] = previewMaterial;
        }
    }

    virtual public void Init(Turret turretOwner, int turretRange) { }

    public void SetDefaultMaterial()
    {
        meshRenderer.materials = defaultMaterials.ToArray();
    }

    public void SetPreviewMaterial()
    {
        meshRenderer.materials = previewMaterials;
    }
}
