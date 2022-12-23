using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyBase : TurretPartBase_Prefab
{

    [SerializeField] private int quantityToIncreaseCurrencyDrop;
    [SerializeField] private Transform topCube;
    [SerializeField] private MeshRenderer cubeMeshRenderer;
    [SerializeField] private Material topCubeMaterial;
    private float positionMovement = 0;
    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);
        //owner = turretOwner;
        turretOwner.OnEnemyEnterRange += increaseCurrencyDrop;
        turretOwner.OnEnemyExitRange += decreaseCurrencyDrop;
    }

    public override void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        base.InitAsSupportBuilding(supportBuilding, supportRange);
        supportBuilding.OnEnemyEnterRange += increaseCurrencyDrop;   
        supportBuilding.OnEnemyExitRange += decreaseCurrencyDrop;
    }

    private void increaseCurrencyDrop(Enemy enemy)
    {
        enemy.currencyDrop += quantityToIncreaseCurrencyDrop;
    }

    private void decreaseCurrencyDrop(Enemy enemy)
    {
        enemy.currencyDrop -= quantityToIncreaseCurrencyDrop;
    }

    private void onEnemyDead()
    {
        //Change emissiveness of topObject
        //PlaySound(?)
    }

    public override void SetDefaultMaterial()
    {
        base.SetDefaultMaterial();
        cubeMeshRenderer.material = topCubeMaterial;
    }

    public override void SetPreviewMaterial()
    {
        base.SetPreviewMaterial();
        cubeMeshRenderer.material = previewMaterials[0];

    }

    public void GlowTopCube()
    {
        //Change Material with more emissiveness
        //cubeMeshRenderer.material.
    }

    // Update is called once per frame
    void Update()
    {
        positionMovement += 0.01f;

        topCube.Rotate(Vector3.up * 0.1f, Space.World);
        topCube.position = new Vector3(topCube.position.x, topCube.position.y + (Mathf.Sin(positionMovement) / 4000.0f), topCube.position.z);
        //Do animation up-down
    }
}
