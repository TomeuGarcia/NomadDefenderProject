using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret : Building
{
    [SerializeField] private GameObject rangePlaneMeshObject;
    private Material rangePlaneMaterial;

    [Header("COMPONENTS")]
    [SerializeField] private CapsuleCollider attackRangeCollider;
    [SerializeField] private Pool attackPool;
    [SerializeField] private MouseOverNotifier meshMouseNotifier;
    private TurretPartBody_Prefab bodyPart;
    private TurretPartBase_Prefab basePart;

    [HideInInspector] public TurretStats stats;
    private float currentShootTimer;

    private List<Enemy> enemies = new List<Enemy>();

    private Vector3 lastTargetedPosition;

    private bool isFunctional = false;

    public delegate void TurretAction(Enemy enemy);
    public TurretAction OnEnemyEnterRange;
    public TurretAction OnEnemyExitRange;
    //public TurretAction OnEnemyShot;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (!enemy.DiesFromQueuedDamage())
            {
                AddEnemy(enemy);
            }  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemies.Contains(enemy))
                RemoveEnemy(enemy);
        }
    }


    public void Init(TurretStats turretStats, TurretCard.TurretCardParts turretCardParts)
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
    public void InitStats(TurretStats stats)
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


    private void AddEnemy(Enemy enemy)
    {
        if (OnEnemyEnterRange != null) OnEnemyEnterRange(enemy);
        enemy.OnEnemyDeactivated += DeleteEnemyFromList;
        enemies.Add(enemy);
        enemies.Sort(mySort);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeactivated -= DeleteEnemyFromList;
        DeleteEnemyFromList(enemy);
    }

    private void DeleteEnemyFromList(Enemy enemyToDelete)
    {
        if (OnEnemyExitRange != null) OnEnemyExitRange(enemyToDelete);
        enemyToDelete.ChangeToBaseMat();
        enemies.Remove(enemyToDelete);

        enemies.Sort(mySort);
    }

    int mySort(Enemy e1, Enemy e2)
    {
        return e1.pathFollower.DistanceLeftToEnd.CompareTo(e2.pathFollower.DistanceLeftToEnd);
    }



    /// <summary>
    /// Returns a List with size up to "amount", assumes there is at least 1 enemy in the turret's queue
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public Enemy[] GetNearestEnemies(int amount, float thresholdDistance)
    { 
        List<Enemy> enemyList = new List<Enemy>();

        enemyList.Add(enemies[0]);
        
        int currentEnemyI = 1;
        while (currentEnemyI < enemies.Count && enemyList.Count < amount)
        {
            if (!enemyList.Contains(enemies[currentEnemyI]) && !enemies[currentEnemyI].DiesFromQueuedDamage())
            {
                float distance = Vector3.Distance(enemyList[enemyList.Count - 1].transformToMove.position, enemies[currentEnemyI].transformToMove.position);
                if (distance < thresholdDistance)
                {
                    enemyList.Add(enemies[currentEnemyI]);
                    currentEnemyI = 0;
                }
            }

            ++currentEnemyI;
        }

        return enemyList.ToArray();
    }


}
