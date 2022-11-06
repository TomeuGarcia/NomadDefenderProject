using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPartBody_Prefab : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] public Transform shootingPointParent;
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

    private int currentShootingPoint = 0;
    public Vector3 GetNextShootingPoint()
    {
        currentShootingPoint++;
        currentShootingPoint %= shootingPointParent.childCount;

        return shootingPointParent.GetChild(currentShootingPoint).position;
    }

    public void SetDefaultMaterial()
    {
        meshRenderer.materials = defaultMaterials.ToArray();
    }

    public void SetPreviewMaterial()
    {
        meshRenderer.materials = previewMaterials;
    }
}
