using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TurretPartBody_Prefab : MonoBehaviour
{
    [SerializeField] public bool lookAtTarget;
    [SerializeField] public Transform shootingPointParent;
    private int currentShootingPoint = 0;


    [SerializeField] private Material previewMaterial;
    [SerializeField] private MeshRenderer[] meshRenderers;
    private Material[][] defaultMaterials;
    private Material[][] previewMaterials;


    [System.Serializable]
    private struct MeshAndMaterialI
    {
        public int meshI, materialI;
    }
    [SerializeField] MeshAndMaterialI[] projectileMaterialIndices;

    [Header("TURRET UPGRADE VISUALS")]
    [SerializeField] private GameObject[] turretUpgradeVisuals;


    public virtual void Init(Material projectileMaterial)
    {
        InitMaterials(projectileMaterial);

        InitTurretUpgradeVisuals(0);
    }

    private void InitMaterials(Material projectileMaterial)
    {
        defaultMaterials = new Material[meshRenderers.Length][];
        previewMaterials = new Material[meshRenderers.Length][];

        for (int meshI = 0; meshI < meshRenderers.Length; ++meshI)
        {
            defaultMaterials[meshI] = new Material[meshRenderers[meshI].materials.Length];
            previewMaterials[meshI] = new Material[meshRenderers[meshI].materials.Length];

            for (int i = 0; i < meshRenderers[meshI].materials.Length; ++i)
            {
                defaultMaterials[meshI][i] = meshRenderers[meshI].materials[i];
                previewMaterials[meshI][i] = previewMaterial;
            }
        }


        ResetProjectileMaterial(projectileMaterial);
    }

    public virtual void ResetProjectileMaterial(Material projectileMaterial)
    {
        // Replace inital materials for projectile material
        for (int i = 0; i < projectileMaterialIndices.Length; ++i)
        {
            defaultMaterials[projectileMaterialIndices[i].meshI][projectileMaterialIndices[i].materialI] = projectileMaterial;
        }
    }

    public Vector3 GetNextShootingPoint()
    {
        currentShootingPoint++;
        currentShootingPoint %= shootingPointParent.childCount;

        return shootingPointParent.GetChild(currentShootingPoint).position;
    }

    public void SetDefaultMaterial()
    {
        for (int meshI = 0; meshI < meshRenderers.Length; ++meshI)
        {
            meshRenderers[meshI].materials = defaultMaterials[meshI];
        }            
    }

    public void SetPreviewMaterial()
    {
        for (int meshI = 0; meshI < meshRenderers.Length; ++meshI)
        {
            meshRenderers[meshI].materials = previewMaterials[meshI];
        }
    }


    public void SetMaterialColor(Color color)
    {
        previewMaterial.color = color;
    }

    private void InitTurretUpgradeVisuals(int level)
    {
        for (int i = 0; i < level; ++i)
        {
            turretUpgradeVisuals[i].SetActive(true);
        }
        for (int i = level; i < turretUpgradeVisuals.Length; ++i)
        {
            turretUpgradeVisuals[i].SetActive(false);
        }
    }

    public void PlayUpgradeAnimation(int level)
    {
        if (level > turretUpgradeVisuals.Length) return;
        turretUpgradeVisuals[level-1].SetActive(true);
    }
}
