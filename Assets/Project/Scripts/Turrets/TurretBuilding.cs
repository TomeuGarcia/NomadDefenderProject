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



    [SerializeField] private GameObject rangePlaneMeshObject;
    private Material rangePlaneMaterial;

    [Header("COMPONENTS")]
    [SerializeField] private CapsuleCollider attackRangeCollider;
    [SerializeField] private Pool attackPool;
    [SerializeField] private MouseOverNotifier meshMouseNotifier;
    private TurretPartBody_Prefab bodyPart;
    private TurretPartBase_Prefab basePart;

    [HideInInspector] public TurretBuildingStats stats;
    private float currentShootTimer;

    private Vector3 lastTargetedPosition;

    private bool isFunctional = false;


    private TurretPartBody.BodyType bodyType;




    private void Awake()
    {
        currentShootTimer = 0.0f;
        //DisableFunctionality();
        HideRangePlane();
    }

    private void Update()
    {
        if (isFunctional)
        {
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
        this.bodyType = turretCardParts.turretPartBody.bodyType;

        float planeRange = stats.range * 2 + 1; //only for square
        float range = stats.range;

        attackRangeCollider.radius = range+ 0.5f;
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

    protected override void DisableFunctionality()
    {
        bodyPart.SetPreviewMaterial();
        basePart.SetPreviewMaterial();

        meshMouseNotifier.gameObject.SetActive(false);
        attackRangeCollider.enabled = false;
        isFunctional = false;
    }

    protected override void EnableFunctionality()
    {
        bodyPart.SetDefaultMaterial();
        basePart.SetDefaultMaterial();
        
        meshMouseNotifier.gameObject.SetActive(true);
        attackRangeCollider.enabled = true;
        isFunctional = true;
    }

    public override void GotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();
    }

    public override void ShowRangePlane()
    {
        rangePlaneMeshObject.SetActive(true);
    }

    public override void HideRangePlane()
    {
        rangePlaneMeshObject.SetActive(false);
    }

    public override void EnablePlayerInteraction()
    {
        meshMouseNotifier.OnMouseEntered += ShowRangePlane;
        meshMouseNotifier.OnMouseExited += HideRangePlane;
    }

    public override void DisablePlayerInteraction()
    {
        meshMouseNotifier.OnMouseEntered -= ShowRangePlane;
        meshMouseNotifier.OnMouseExited -= HideRangePlane;
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

}
