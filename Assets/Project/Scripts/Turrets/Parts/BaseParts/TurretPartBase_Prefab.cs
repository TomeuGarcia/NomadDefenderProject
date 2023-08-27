using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretPartBase_Prefab : MonoBehaviour
{
    [SerializeField] private Transform meshTransform;
    public Transform MeshTransform => meshTransform;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] protected Material previewMaterial;
    private Material[] defaultMaterials;
    protected Material[] previewMaterials;

    [Header("BASE COLLIDER")]
    [SerializeField] public BaseCollider baseCollider;
    [SerializeField] GameObject[] visualUpgrades;

    [Header("PARTICLES")]
    [SerializeField] protected ParticleSystem placedParticleSystem;
    public ParticleSystem PlacedParticleSystem => placedParticleSystem;


    private void Awake()
    {
        AwakeInit();
    }
    protected void AwakeInit()
    {
        foreach (GameObject go in visualUpgrades)
        {
            go.SetActive(false);
        }
    }

    virtual public void Init(TurretBuilding turretOwner, float turretRange)
    {
        InitMaterials();
        
    }
    virtual public void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        InitMaterials();
    }

    virtual public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel) 
    {
        if (newStatLevel <= visualUpgrades.Length) visualUpgrades[newStatLevel - 1].SetActive(true);
    }

    protected virtual void InitMaterials()
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

    public bool IsPointWithinRange(Vector3 point)
    {
        return baseCollider.IsPointWithinRange(point);
    }

    public virtual void OnGetPlaced()
    {

    }
    public virtual void GotEnabledPlacing()
    {

    }
    public virtual void GotDisabledPlacing()
    {

    }
    public virtual void GotMovedWhenPlacing()
    {

    }
    
    public virtual void GotHoveredWhenPlaced()
    {
    }
    public virtual void GotUnoveredWhenPlaced()
    {
    }


    public void SetMaterialColor(Color color)
    {
        previewMaterial.color = color;
        baseCollider.SetRangeColor(color);
    }

    protected void UpdateAreaPlaneSize(SupportBuilding supportOwner, MeshRenderer specialAreaPlaneMesh, Material specialAreaPlaneMaterial)
    {
        float planeRange = supportOwner.stats.range * 2 + 1; //only for square
        float range = supportOwner.stats.range;

        specialAreaPlaneMesh.transform.localScale = Vector3.one * ((float)planeRange / 10.0f);
        specialAreaPlaneMaterial.SetFloat("_TileNum", planeRange);
    }

}
