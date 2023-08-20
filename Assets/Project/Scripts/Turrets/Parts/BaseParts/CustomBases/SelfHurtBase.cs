using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHurtBase : TurretPartBase_Prefab
{
    [Header("SELF HURT BASE")]
    [SerializeField] private int[] explosionDamagePerUpgrade;
    private int explosionDamage;


    [SerializeField] private MeshRenderer rangePlane;
    private Material rangePlaneMaterial;

    [SerializeField] private MeshRenderer explosionCapsuleMesh;
    private Material explosionCapsuleMaterial;


    private List<Enemy> enemies = new List<Enemy>();

    private int currentLvl = 0;

    RangeBuilding owner;


    private void Awake()
    {
        AwakeInit();

        rangePlane.gameObject.SetActive(false);
        explosionCapsuleMesh.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        owner.OnEnemyEnterRange -= AddEnemy;
        owner.OnEnemyExitRange -= RemoveEnemy;

        PathLocation.OnTakeDamage -= OnPathLocationTakesDamage;
    }

    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);
        
        owner = turretOwner;
        owner.OnEnemyEnterRange += AddEnemy;
        owner.OnEnemyExitRange += RemoveEnemy;

        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;

        explosionDamage = explosionDamagePerUpgrade[currentLvl];

        explosionCapsuleMaterial = explosionCapsuleMesh.material;
    }

    public override void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        base.InitAsSupportBuilding(supportBuilding, supportRange);

        owner = supportBuilding;
        owner.OnEnemyEnterRange += AddEnemy;
        owner.OnEnemyExitRange += RemoveEnemy;

        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;

        explosionDamage = explosionDamagePerUpgrade[currentLvl];

        float planeRange = supportBuilding.stats.range * 2 + 1; //only for square
        float range = supportBuilding.stats.range;

        rangePlane.transform.localScale = Vector3.one * (planeRange / 10f);
        rangePlaneMaterial = rangePlane.material;
        rangePlaneMaterial.SetFloat("_TileNum", planeRange);

        explosionCapsuleMaterial = explosionCapsuleMesh.material;
    }

    public override void OnGetPlaced()
    {
        rangePlane.gameObject.SetActive(true);
    }

    override public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatnewStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatnewStatLevel);

        explosionDamage = explosionDamagePerUpgrade[currentLvl];
    }


    private void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }



    public override void SetDefaultMaterial()
    {
        base.SetDefaultMaterial();
    }

    public override void SetPreviewMaterial()
    {
        base.SetPreviewMaterial();

    }

    private void OnPathLocationTakesDamage(PathLocation pathLocation)
    {
        DamageEnemies();
        PlayExplosionAnimation();
    }

    private void DamageEnemies()
    {
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


}
