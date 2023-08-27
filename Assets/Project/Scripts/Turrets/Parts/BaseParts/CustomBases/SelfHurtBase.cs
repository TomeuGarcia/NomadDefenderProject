using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SelfHurtBase : TurretPartBase_Prefab
{
    [Header("SELF HURT BASE")]
    [SerializeField, Range(1, 5)] private int pathLocationDamage = 1;
    [SerializeField, Min(0)] private int baseExplosionDamage;
    [SerializeField] private float[] explosionDamagePer1Multiplier;
    private int explosionDamage;


    [Header("MESHES")]
    [SerializeField] private MeshRenderer rangePlane;
    private Material rangePlaneMaterial;

    [SerializeField] private MeshRenderer explosionCapsuleMesh;
    private Material explosionCapsuleMaterial;

    [SerializeField] private Transform bindOriginTransform;
    [SerializeField] private MeshRenderer nodeBinderMesh;



    private int currentLvl = 0;

    RangeBuilding owner;


    private void Awake()
    {
        AwakeInit();

        rangePlane.gameObject.SetActive(false);
        explosionCapsuleMesh.gameObject.SetActive(false);
        HideNodeBinder();
    }

    private void OnDestroy()
    {
        owner.OnPlaced -= OnOwnerBuildingPlaced;

        PathLocation.OnTakeDamage -= OnPathLocationTakesDamage;
    }

    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);
        
        owner = turretOwner;
        owner.OnPlaced += OnOwnerBuildingPlaced;

        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;

        UpdateExplosionDamage();

        explosionCapsuleMesh.gameObject.SetActive(true);
        explosionCapsuleMaterial = explosionCapsuleMesh.material;
    }

    public override void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        base.InitAsSupportBuilding(supportBuilding, supportRange);

        owner = supportBuilding;
        owner.OnPlaced += OnOwnerBuildingPlaced;

        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;

        UpdateExplosionDamage();

        float planeRange = supportBuilding.stats.range * 2 + 1; //only for square
        float range = supportBuilding.stats.range;

        rangePlane.transform.localScale = Vector3.one * (planeRange / 10f);
        rangePlaneMaterial = rangePlane.material;
        rangePlaneMaterial.SetFloat("_TileNum", planeRange);

        explosionCapsuleMesh.gameObject.SetActive(true);
        explosionCapsuleMaterial = explosionCapsuleMesh.material;
    }

    public override void OnGetPlaced()
    {
        rangePlane.gameObject.SetActive(true);
    }

    override public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatLevel);

        UpdateExplosionDamage();

        if (newStatLevel == 3)
        {
            ownerSupportBuilding.UpgradeRangeIncrementingLevel();
            UpdateAreaPlaneSize(ownerSupportBuilding, rangePlane, rangePlaneMaterial);
        }
    }

    private void UpdateExplosionDamage()
    {
        explosionDamage = (int)(baseExplosionDamage * explosionDamagePer1Multiplier[currentLvl]);
    }



    public override void SetDefaultMaterial()
    {
        base.SetDefaultMaterial();
    }

    public override void SetPreviewMaterial()
    {
        base.SetPreviewMaterial();

    }


    private async void OnOwnerBuildingPlaced(Building invokerBuilding)
    {
        await Task.Delay(200);
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation();
        pathLocation.TakeDamage(pathLocationDamage);
    }

    private void OnPathLocationTakesDamage(PathLocation pathLocation)
    {
        ConnectBinderWithPathLocation();

        DamageEnemies();
        PlayExplosionAnimation();
    }

    private void DamageEnemies()
    {
        Enemy[] enemies = owner.GetEnemiesInRange();
        foreach (Enemy enemy in enemies)
        {
            enemy.QueueDamage(explosionDamage);
            enemy.TakeDamage(null, explosionDamage);
        }
    }

    private void PlayExplosionAnimation()
    {
        explosionCapsuleMaterial.SetFloat("_StartTimeFlashAnimation", Time.time);
    }




    public override void GotEnabledPlacing()
    {
        ConnectBinderWithPathLocation();
    }
    public override void GotDisabledPlacing()
    {
        HideNodeBinder();
    }
    public override void GotMovedWhenPlacing()
    {
        ConnectBinderWithPathLocation();
    }



    private void ShowNodeBinder()
    {
        nodeBinderMesh.gameObject.SetActive(true);
    }
    private void HideNodeBinder()
    {
        nodeBinderMesh.gameObject.SetActive(false);
    }

    private void ConnectBinderWithPathLocation()
    {
        ShowNodeBinder();
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation();
        TurretBinderUtils.UpdateTurretBinder(nodeBinderMesh.transform, pathLocation.transform, bindOriginTransform);
    }
}
