using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPartBody_View : MonoBehaviour
{
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

    public void InitMaterials(Material projectileMaterial)
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

    public void ResetProjectileMaterial(Material projectileMaterial)
    {
        // Replace inital materials for projectile material

        for (int i = 0; i < projectileMaterialIndices.Length; ++i)
        {
            defaultMaterials[projectileMaterialIndices[i].meshI][projectileMaterialIndices[i].materialI] = projectileMaterial;
        }
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
}
