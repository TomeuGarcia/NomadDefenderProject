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
    private Material topCubeMaterial;
    [SerializeField] private GameObject currencyPlane;
    private Material currencyPlaneMaterial;
    [SerializeField] private float duration;

    private float positionMovement = 0;

    private void Awake()
    {
        currencyPlane.SetActive(false);
        topCubeMaterial = cubeMeshRenderer.material;
    }
    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);
        //owner = turretOwner;
        turretOwner.OnEnemyEnterRange += increaseCurrencyDrop;
        turretOwner.OnEnemyExitRange += decreaseCurrencyDrop;
        turretOwner.OnEnemyEnterRange += EnemyBloomsCube;
        turretOwner.OnEnemyExitRange += EnemyDoesNotBloomCube;

    }

    public override void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        base.InitAsSupportBuilding(supportBuilding, supportRange);
        supportBuilding.OnEnemyEnterRange += increaseCurrencyDrop;   
        supportBuilding.OnEnemyExitRange += decreaseCurrencyDrop;
        supportBuilding.OnEnemyEnterRange += EnemyBloomsCube;
        supportBuilding.OnEnemyExitRange += EnemyDoesNotBloomCube;

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

    // Update is called once per frame
    void Update()
    {
        positionMovement += 0.01f;

        topCube.Rotate(Vector3.up * 0.1f, Space.World);
        topCube.position = new Vector3(topCube.position.x, topCube.position.y + (Mathf.Sin(positionMovement) / 4000.0f), topCube.position.z);
        //Do animation up-down
    }

    public void DoBloomCube(Enemy enemy)
    {
        StartCoroutine(BloomCube());
    }

    public void EnemyBloomsCube(Enemy enemy)
    {
        enemy.OnEnemyDeath += DoBloomCube;
    }

    public void EnemyDoesNotBloomCube(Enemy enemy)
    {
        enemy.OnEnemyDeath -= DoBloomCube;
    }

    IEnumerator BloomCube()
    {
        float tParam = 0.0f;

        do
        {

            tParam += Time.deltaTime;
            topCubeMaterial.SetFloat("_lerpTime", tParam / duration);
            yield return null;

        } while (tParam < duration);


        yield return null;

        do
        {
            tParam -= Time.deltaTime;
            topCubeMaterial.SetFloat("_lerpTime", tParam / duration);
            yield return null;

        } while (tParam >= 0.0f);

        topCubeMaterial.SetFloat("_lerpTime", 0.0f);
    }
}
