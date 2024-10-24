using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TurretPartBody_Prefab : MonoBehaviour
{
    [SerializeField] public TurretPartBody_View _view;
    [SerializeField] public Transform binderPoint;
    [SerializeField] private Transform _addOnsParent;
    
    [SerializeField] public Transform shootingPointParent;
    private int currentShootingPoint = 0;
    private Material[][] defaultMaterials;
    private Material[][] previewMaterials;

    
    [Header("TURRET UPGRADE VISUALS")]
    [SerializeField] private GameObject[] turretUpgradeVisuals;

    public TurretPartBody.BodyType BodyType { get; private set; }
    public Transform AddOnsParent => _addOnsParent;


    public virtual void Init(TurretPartBody.BodyType bodyType, Material projectileMaterial)
    {
        BodyType = bodyType;
        _view.InitMaterials(projectileMaterial);

        ResetUpgradeVisuals();
    }

    public virtual void ResetProjectileMaterial(Material projectileMaterial)
    {
        _view.ResetProjectileMaterial(projectileMaterial);
    }

    public Vector3 GetNextShootingPoint()
    {
        currentShootingPoint++;
        currentShootingPoint %= shootingPointParent.childCount;

        return shootingPointParent.GetChild(currentShootingPoint).position;
    }

    public void SetDefaultMaterial()
    {
        _view.SetDefaultMaterial();
    }

    public void SetPreviewMaterial()
    {
        _view.SetPreviewMaterial();
    }


    public void SetMaterialColor(Color color)
    {
        _view.SetMaterialColor(color);
    }

    public void ResetUpgradeVisuals()
    {
        InitTurretUpgradeVisuals(0);
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
        if (level > turretUpgradeVisuals.Length || level <= 0) return;
        turretUpgradeVisuals[level-1].SetActive(true);
    }
}
