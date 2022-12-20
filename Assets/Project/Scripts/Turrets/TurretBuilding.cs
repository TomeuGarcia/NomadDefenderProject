using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("COLLIDER")]
    [SerializeField] private CapsuleCollider rangeCollider; // Might want to make this depend on BasePart in the future

    [Header("ATTACK POOL")]
    [SerializeField] private Pool attackPool;

    private TurretPartBody_Prefab bodyPart;
    private TurretPartBase_Prefab basePart;

    private float currentShootTimer;
    private Vector3 lastTargetedPosition;

    private TurretPartBody.BodyType bodyType; // Used to play sound

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;

    void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        currentShootTimer = 0.0f;        
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

    public void Init(TurretBuildingStats turretStats, TurretBuildingCard.TurretCardParts turretCardParts)
    {
        TurretPartAttack turretPartAttack = turretCardParts.turretPartAttack;
        TurretPartBody turretPartBody = turretCardParts.turretPartBody;
        TurretPartBase turretPartBase = turretCardParts.turretPartBase;

        InitStats(turretStats);
        bodyType = turretCardParts.turretPartBody.bodyType;


        float planeRange = stats.range * 2 + 1; //only for square
        float range = stats.range;

        rangeCollider.radius = range+ 0.5f;
        rangePlaneMeshObject.transform.localScale = Vector3.one * (planeRange / 10f);
        rangePlaneMaterial = rangePlaneMeshObject.GetComponent<MeshRenderer>().materials[0];
        rangePlaneMaterial.SetFloat("_TileNum", planeRange);


        TurretPartAttack_Prefab turretAttack = turretPartAttack.prefab.GetComponent<TurretPartAttack_Prefab>();
        attackPool.SetPooledObject(turretPartAttack.prefab);

        bodyPart = Instantiate(turretPartBody.prefab, bodyHolder).GetComponent<TurretPartBody_Prefab>();
        bodyPart.Init(turretAttack.materialForTurret);

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.Init(this, range);


        DisableFunctionality();
    }

    public void InitStats(TurretBuildingStats stats)
    {
        this.stats = stats;
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
        currentAttack.transform.position = bodyPart.GetNextShootingPoint();
        currentAttack.transform.parent = attackPool.transform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.Init(enemyTarget, this);

        enemyTarget.ChangeMat();

        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(bodyType);
    }



    protected override void DisableFunctionality()
    {
        base.DisableFunctionality();

        rangeCollider.enabled = false;

        bodyPart.SetPreviewMaterial();
        basePart.SetPreviewMaterial();
    }

    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();

        rangeCollider.enabled = true;

        bodyPart.SetDefaultMaterial();
        basePart.SetDefaultMaterial();
    }

    public override void GotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();
    }

}
