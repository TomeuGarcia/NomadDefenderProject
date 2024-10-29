using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretPartBase_Prefab : MonoBehaviour
{
    [SerializeField] private Transform meshTransform;
    public Transform MeshTransform => meshTransform;
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] protected Material previewMaterial;
    private Material[][] defaultMaterials;
    protected Material[][] previewMaterials;

    [Header("BASE COLLIDER")]
    [SerializeField] public BaseCollider baseCollider;
    [SerializeField] GameObject[] visualUpgrades;
    
    public Vector3 PlacedParticlesSpot => meshTransform.position;

    public Collider Collider => baseCollider.GetCollider();
    
    protected bool AbilityIsDisabled { get; private set; }


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

    public virtual void Init(TurretBuilding turretOwner, float turretRange)
    {
        InitMaterials();
        
    }
    public virtual void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        InitMaterials();
    }

    public virtual void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel) 
    {
        if (newStatLevel <= visualUpgrades.Length) visualUpgrades[newStatLevel - 1].SetActive(true);
    }

    public void ResetUpgrades()
    {
        foreach (GameObject visualUpgrade in visualUpgrades)
        {
            visualUpgrade.SetActive(false);
        }
    }

    protected virtual void InitMaterials()
    {
        defaultMaterials = new Material[meshRenderers.Length][];
        previewMaterials = new Material[meshRenderers.Length][];

        for (int meshI = 0; meshI < meshRenderers.Length; ++meshI)
        {
            MeshRenderer meshRenderer = meshRenderers[meshI];

            defaultMaterials[meshI] = new Material[meshRenderer.materials.Length];
            previewMaterials[meshI] = new Material[meshRenderer.materials.Length];

            for (int i = 0; i < meshRenderer.materials.Length; ++i)
            {
                defaultMaterials[meshI][i] = meshRenderer.materials[i];
                previewMaterials[meshI][i] = previewMaterial;
            }
        }        
    }

    public virtual void SetDefaultMaterial()
    {
        for (int meshI = 0; meshI < meshRenderers.Length; ++meshI)
        {
            meshRenderers[meshI].materials = defaultMaterials[meshI];
        }
    }

    public virtual void SetPreviewMaterial()
    {
        for (int meshI = 0; meshI < meshRenderers.Length; ++meshI)
        {
            meshRenderers[meshI].materials = previewMaterials[meshI];
        }
    }



    public virtual void OnGetPlaced()
    {

    }
    public virtual void OnGetUnplaced()
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

    public virtual void ResetAreaPlaneSize(SupportBuilding supportOwner)
    {
        
    }
    protected void UpdateAreaPlaneSize(SupportBuilding supportOwner, MeshRenderer specialAreaPlaneMesh, Material specialAreaPlaneMaterial)
    {
        float planeRange = supportOwner.Stats.RadiusRange * 2 + 1; //only for square
        float range = supportOwner.Stats.RadiusRange;

        specialAreaPlaneMesh.transform.localScale = Vector3.one * ((float)planeRange / 10.0f);
        specialAreaPlaneMaterial.SetFloat("_TileNum", planeRange);
    }
    

    public virtual void DoOnBuildingDisableStart()
    {
        AbilityIsDisabled = true;
    }

    public virtual void DoOnBuildingDisableFinish()
    {
        AbilityIsDisabled = false;
    }

}
