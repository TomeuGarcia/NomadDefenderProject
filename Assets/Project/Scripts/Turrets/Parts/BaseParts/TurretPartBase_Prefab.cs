using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretPartBase_Prefab : MonoBehaviour
{
    [SerializeField] private Transform meshTransform;
    public Transform MeshTransform => meshTransform;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material previewMaterial;
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
        foreach (GameObject go in visualUpgrades)
        {
            go.SetActive(false);
        }
    }
    virtual public void Init(TurretBuilding turretOwner, float turretRange)
    {
        InitMaterials();
        
    }
    virtual public void InitAsSupportBuilding(SupportBuilding supportBuilding,float supportRange)
    {
        InitMaterials();
    }

    virtual public void Upgrade(int newStatLevel) 
    {
        if (newStatLevel < visualUpgrades.Length) visualUpgrades[newStatLevel - 1].SetActive(true);
    }

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
