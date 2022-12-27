using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static SupportBuilding;

public class CurrencyBase : TurretPartBase_Prefab
{

    [SerializeField] private int quantityToIncreaseCurrencyDrop;
    [SerializeField] private Transform topCube;
    [SerializeField] private MeshRenderer cubeMeshRenderer;
    [SerializeField] private Material topCubeMaterial;
    [SerializeField] private GameObject currencyPlane;
    private Material currencyPlaneMaterial;

    private float positionMovement = 0;

    private void Awake()
    {
        currencyPlane.SetActive(false);
    }
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

        float planeRange = supportBuilding.stats.range * 2 + 1; //only for square
        float range = supportBuilding.stats.range;


        currencyPlane.transform.localScale = Vector3.one * (planeRange / 10f);
        currencyPlaneMaterial = currencyPlane.GetComponent<MeshRenderer>().materials[0];
        currencyPlaneMaterial.SetFloat("_TileNum", planeRange);
    }

    public override void OnGetPlaced()
    {
        currencyPlane.SetActive(true);
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
