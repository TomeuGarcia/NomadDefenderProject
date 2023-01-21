using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretPartBase_Prefab : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material previewMaterial;
    private Material[] defaultMaterials;
    protected Material[] previewMaterials;

    [Header("BASE COLLIDER")]
    [SerializeField] public BaseCollider baseCollider;


    virtual public void Init(TurretBuilding turretOwner, float turretRange)
    {
        InitMaterials();
    }
    virtual public void InitAsSupportBuilding(SupportBuilding supportBuilding,float supportRange)
    {
        InitMaterials();
    }

    virtual public void Upgrade(int newStatLevel) { }

    private void InitMaterials()
    {
        defaultMaterials = new Material[meshRenderer.materials.Length];
        previewMaterials = new Material[meshRenderer.materials.Length];

        for (int i = 0; i < meshRenderer.materials.Length; ++i)
        {
            defaultMaterials[i] = meshRenderer.materials[i];
            previewMaterials[i] = previewMaterial;
        }
    }

    public virtual void SetDefaultMaterial()
    {
        meshRenderer.materials = defaultMaterials;
    }

    public virtual void SetPreviewMaterial()
    {
        meshRenderer.materials = previewMaterials;
    }

    public virtual void OnGetPlaced()
    {

    }

}
