using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretPartBody_View : MonoBehaviour
{
    [SerializeField] private Material _previewMaterial;
    [SerializeField] private TurretBodyMaterialAssigner[] _materialAssigners_Normal;
    [SerializeField] private TurretBodyMaterialAssigner[] _materialAssigners_Lvl1;
    [SerializeField] private TurretBodyMaterialAssigner[] _materialAssigners_Lvl2;
    [SerializeField] private TurretBodyMaterialAssigner[] _materialAssigners_Lvl3;
    private TurretBodyMaterialAssigner[] _allMaterialAssigners;
    
    
    private Material _defaultMaterial;
    
    

    public void InitMaterials(Material projectileMaterial)
    {
        _allMaterialAssigners = 
            _materialAssigners_Normal
            .Union(_materialAssigners_Lvl1)
            .Union(_materialAssigners_Lvl2)
            .Union(_materialAssigners_Lvl3)
            .ToArray();
        
        foreach (TurretBodyMaterialAssigner materialAssigner in _allMaterialAssigners)
        {
            materialAssigner.Init();
        }
        
        ResetProjectileMaterial(projectileMaterial);
    }

    public void ResetProjectileMaterial(Material projectileMaterial)
    {
        _defaultMaterial = projectileMaterial;
        SetDefaultMaterial();
    }

    public void SetDefaultMaterial()
    {
        foreach (TurretBodyMaterialAssigner materialAssigner in _allMaterialAssigners)
        {
            materialAssigner.Assign(_defaultMaterial);
        }
    }

    public void SetPreviewMaterial()
    {
        foreach (TurretBodyMaterialAssigner materialAssigner in _allMaterialAssigners)
        {
            materialAssigner.AssignToAll(_previewMaterial);
        }
    }


    public void SetMaterialColor(Color color)
    {
        _previewMaterial.color = color;
    }
}
