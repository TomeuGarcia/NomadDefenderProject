using DG.Tweening;
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
    [SerializeField] private Transform rotatingMesh;
    [SerializeField] private MeshRenderer rangePlane;
    private Material rangePlaneMaterial;

    [SerializeField] private MeshRenderer explosionCapsuleMesh;
    private Material explosionCapsuleMaterial;

    [SerializeField] private Transform bindOriginTransform;
    [SerializeField] private MeshRenderer nodeBinderMesh;

    [Header("UPGRADES EXTRA")]
    [SerializeField] private Transform rocketTop;
    [SerializeField] private Vector3 rocketTopMoveBy = Vector3.up * 0.4f;


    private bool isPlaced = false;



    private int currentLvl = 0;

    RangeBuilding owner;


    private void Awake()
    {
        AwakeInit();

        rangePlane.gameObject.SetActive(false);
        explosionCapsuleMesh.gameObject.SetActive(false);
        HideNodeBinder();

        isPlaced = false;
    }

    private void OnDestroy()
    {
        owner.OnPlaced -= OnOwnerBuildingPlaced;

        if (isPlaced)
        {
            PathLocation.OnTakeDamage -= OnPathLocationTakesDamage;
        }

        rotatingMesh.DOKill();
    }

    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);
        
        owner = turretOwner;
        owner.OnPlaced += OnOwnerBuildingPlaced;


        UpdateExplosionDamage();

        explosionCapsuleMesh.gameObject.SetActive(true);
        explosionCapsuleMaterial = explosionCapsuleMesh.material;
    }

    public override void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        base.InitAsSupportBuilding(supportBuilding, supportRange);

        owner = supportBuilding;
        owner.OnPlaced += OnOwnerBuildingPlaced;

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
        isPlaced = true;
        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;

        rangePlane.gameObject.SetActive(true);
        HideNodeBinder();
        rotatingMesh.DOBlendableRotateBy(Vector3.up * 180f, 8.0f).SetLoops(-1);
    }

    override public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatLevel);

        UpdateExplosionDamage();

        if (newStatLevel == 3)
        {
            ownerSupportBuilding.UpgradeRangeIncrementingLevel();
            UpdateAreaPlaneSize(ownerSupportBuilding, rangePlane, rangePlaneMaterial);

            rocketTop.DOBlendableMoveBy(rocketTopMoveBy, 1.0f);
        }
    }

    private void UpdateExplosionDamage()
    {
        explosionDamage = (int)(baseExplosionDamage * explosionDamagePer1Multiplier[currentLvl]);
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

        DamageEnemies(0.3f);
        PlayExplosionAnimation();

        GameAudioManager.GetInstance().PlaySelfHurtExplosion();
    }

    private async void DamageEnemies(float delay)
    {
        Enemy[] enemies = owner.GetEnemiesInRange();

        await Task.Delay((int)(delay * 1000));

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
        ShowNodeBinder();
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
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation();
        TurretBinderUtils.UpdateTurretBinder(nodeBinderMesh.transform, pathLocation.transform, bindOriginTransform);
    }
}
