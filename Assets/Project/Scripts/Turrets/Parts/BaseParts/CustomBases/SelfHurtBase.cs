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
            owner.OnShowRangePlane -= OnRangePlaneShown;
            owner.OnHideRangePlane -= OnRangePlaneHidden;
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

        float planeRange = supportBuilding.Stats.RadiusRange * 2 + 1; //only for square
        float range = supportBuilding.Stats.RadiusRange;

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
        owner.OnShowRangePlane += OnRangePlaneShown;
        owner.OnHideRangePlane += OnRangePlaneHidden;

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

        DamageHealthiestLocation(pathLocationDamage);
    }

    private void UpdateExplosionDamage()
    {
        explosionDamage = (int)(baseExplosionDamage * explosionDamagePer1Multiplier[currentLvl]);
        
    }



    private void OnOwnerBuildingPlaced(Building invokerBuilding)
    {
        DamageHealthiestLocation(pathLocationDamage);
    }

    private async void DamageHealthiestLocation(int damage)
    {
        await Task.Delay(200);

        if (ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation(owner.Position, out PathLocation pathLocation))
        {
            pathLocation.TakeDamage(damage);
        }        
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
            TurretDamageAttack explosionDamageAttack = new TurretDamageAttack(null, enemy, explosionDamage);
            enemy.QueueDamage(explosionDamageAttack);
            enemy.TakeDamage(explosionDamageAttack, DamageEnemiesResult);
        }
    }

    private void DamageEnemiesResult(TurretDamageAttackResult damageAttackResult)
    {
        
    }

    private void PlayExplosionAnimation()
    {
        explosionCapsuleMaterial.SetFloat("_StartTimeFlashAnimation", Time.time);
    }




    public override void GotEnabledPlacing()
    {
        ShowNodeBinder();
        ConnectBinderWithPathLocation();
        PathLocation.OnHealthChanged += ConnectBinderWithPathLocation;
    }
    public override void GotDisabledPlacing()
    {
        HideNodeBinder();
        PathLocation.OnHealthChanged -= ConnectBinderWithPathLocation;
    }
    public override void GotMovedWhenPlacing()
    {
        ConnectBinderWithPathLocation();
    }

    private void OnRangePlaneShown()
    {
        ShowNodeBinder();
        ConnectBinderWithPathLocation();
    }
    private void OnRangePlaneHidden()
    {
        HideNodeBinder();
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
        if (ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation(transform.position, out PathLocation healthiestLocation))
        {
            TurretBinderUtils.UpdateTurretBinder(nodeBinderMesh.transform, healthiestLocation.transform, bindOriginTransform);
        }               
    }
}
