using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class TurretBuilding : RangeBuilding
{
    public struct TurretBuildingStats
    {
        public int playCost;
        public int damage;
        [SerializeField, Min(1)] public float range;
        public float cadence;
    }

    [HideInInspector] public TurretBuildingStats stats;

    [Header("ATTACK POOL")]
    [SerializeField] private Pool attackPool;

    private TurretPartBody_Prefab bodyPart;

    public PassiveDamageModifier baseDamagePassive;

    private float currentShootTimer;
    private Vector3 lastTargetedPosition;

    private TurretPartBody.BodyType bodyType; // Used to play sound

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;

    [Header("PARTICLES")]
    [SerializeField] protected ParticleSystem placedParticleSystem;


    public int CardLevel { get; private set; }


    public const int MIN_PLAY_COST = 50;


    void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        currentShootTimer = 0.0f;
        placedParticleSystem.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isFunctional) return;

        UpdateShoot();
        if (bodyPart.lookAtTarget)
        {
            if (enemies.Count > 0)
            {
                lastTargetedPosition = enemies[0].transform.position;
            }
            LookAtTarget();
        }
    }


    private void LookAtTarget()
    {
        Quaternion targetRot = Quaternion.LookRotation((lastTargetedPosition - bodyPart.transform.position).normalized, bodyPart.transform.up);
        bodyPart.transform.rotation = Quaternion.RotateTowards(bodyPart.transform.rotation, targetRot, 600.0f * Time.deltaTime);
    }

    public void Init(TurretBuildingStats turretStats, TurretCardParts turretCardParts, CurrencyCounter currencyCounter)
    {
        TurretPartAttack turretPartAttack = turretCardParts.turretPartAttack;
        TurretPartBody turretPartBody = turretCardParts.turretPartBody;
        TurretPartBase turretPartBase = turretCardParts.turretPartBase;

        CardLevel = turretCardParts.cardLevel;

        InitStats(turretStats);
        bodyType = turretCardParts.turretPartBody.bodyType;


        TurretPartAttack_Prefab turretAttack = turretPartAttack.prefab.GetComponent<TurretPartAttack_Prefab>();
        attackPool.SetPooledObject(turretPartAttack.prefab);

        bodyPart = Instantiate(turretPartBody.prefab, bodyHolder).GetComponent<TurretPartBody_Prefab>();
        bodyPart.Init(turretAttack.materialForTurret);

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.Init(this, stats.range);

        UpdateRange();
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);

        //PASSIVE
        turretCardParts.turretPassiveBase.passive.ApplyEffects(this);

        upgrader.InitTurret(turretPartBody.damageLvl, turretPartBody.cadenceLvl, turretPartBase.rangeLvl, currencyCounter);

        DisableFunctionality();
    }

    protected override void UpdateRange()
    {
        basePart.baseCollider.UpdateRange(stats.range);
    }


    public void InitStats(TurretBuildingStats stats)
    {
        this.stats = stats;
    }

    public override void Upgrade(TurretUpgradeType upgradeType, int newStatLevel)
    {
        switch(upgradeType)
        {
            case TurretUpgradeType.ATTACK:
                stats.damage = TurretPartBody.damagePerLvl[newStatLevel - 1];
                break;
            case TurretUpgradeType.CADENCE:
                stats.cadence = TurretPartBody.cadencePerLvl[newStatLevel - 1];
                break;
            case TurretUpgradeType.RANGE:
                stats.range = TurretPartBase.rangePerLvl[newStatLevel - 1];
                UpdateRange();
                break;
        }
    }

    private void UpdateShoot()
    {
        if (currentShootTimer < stats.cadence)
        {
            currentShootTimer += Time.deltaTime;
            return;
        }

        if (enemies.Count <= 0) return;

        currentShootTimer = 0.0f;

        DoShootEnemyLogic(enemies[0]);


        //// Code used when turrets used to have targetAmount stat:
        /*
        for (int i = 0; i < stats.targetAmount; i++)
        {
            if (i <= enemies.Count - 1)
            {
                DoShootEnemyLogic(enemies[i]);
            }
        }
        */
    }

    private void DoShootEnemyLogic(Enemy enemyTarget)
    {
        if (enemyTarget.DiesFromQueuedDamage())
        {
            RemoveEnemy(enemyTarget);
        }
        else
        {
            Shoot(enemyTarget);
            bodyPart.transform.DOPunchPosition(bodyPart.transform.forward * -0.1f, 0.25f, 5, 1.0f, false);
        }
    }

    private void Shoot(Enemy enemyTarget)
    {
        TurretPartAttack_Prefab currentAttack = attackPool.GetObject().gameObject.GetComponent<TurretPartAttack_Prefab>();
        Vector3 shootPoint = bodyPart.GetNextShootingPoint();
        currentAttack.transform.position = shootPoint;
        currentAttack.transform.parent = attackPool.transform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.Init(enemyTarget, this);


        // Spawn particle
        GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(currentAttack.GetAttackType, shootPoint, bodyPart.transform.rotation);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;


        enemyTarget.ChangeMat();

        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(bodyType);
    }



    protected override void DisableFunctionality()
    {
        base.DisableFunctionality();

        basePart.baseCollider.DisableCollisions();

        bodyPart.SetPreviewMaterial();
        basePart.SetPreviewMaterial();
    }

    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();

        basePart.baseCollider.EnableCollisions();

        bodyPart.SetDefaultMaterial();
        basePart.SetDefaultMaterial();
    }

    public override void GotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();

        bodyHolder.DOPunchScale(Vector3.up * -0.3f, 0.7f, 7);
     
        placedParticleSystem.gameObject.SetActive(true);
        placedParticleSystem.Play();
    }

}
