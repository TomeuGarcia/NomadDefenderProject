using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret : Building
{
    [SerializeField] private GameObject rangePlaneMeshObject;
    private Material rangePlaneMaterial;

    [Header("COMPONENTS")]
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Pool attackPool;
    [SerializeField] private MouseOverNotifier meshMouseNotifier;
    private TurretPartBody_Prefab bodyPart;
    private TurretPartBase_Prefab basePart;

    private TurretStats stats;
    private float currentShootTimer;

    private List<Enemy> enemies = new List<Enemy>();

    private bool isFunctional = false;

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
        }
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


    public override void Init(TurretStats turretStats, GameObject turretAttack, GameObject turretPartBody, GameObject turretPartBase)
    {
        InitStats(turretStats);

        int range = stats.range * 2 + 1;

        boxCollider.size = new Vector3(range, 1.0f, range);
        rangePlaneMeshObject.transform.localScale = Vector3.one * (range / 10f);
        rangePlaneMaterial = rangePlaneMeshObject.GetComponent<MeshRenderer>().materials[0];
        rangePlaneMaterial.SetFloat("_TileNum", (float)range);

        bodyPart = Instantiate(turretPartBody, bodyHolder).GetComponent<TurretPartBody_Prefab>();
        basePart = Instantiate(turretPartBase, baseHolder).GetComponent<TurretPartBase_Prefab>();

        attackPool.SetPooledObject(turretAttack);

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

        for (int i = 0; i < stats.targetAmount; i++)
        {
            if (i <= enemies.Count - 1)
            {
                if (enemies[i].DiesFromQueuedDamage())
                {
                    RemoveEnemy(enemies[i]);
                    --i;
                }
                else
                {
                    Shoot(enemies[i]);
                }
            }
        }

    }

    protected override void DisableFunctionality()
    {
        bodyPart.SetPreviewMaterial();
        basePart.SetPreviewMaterial();

        meshMouseNotifier.gameObject.SetActive(false);
        boxCollider.enabled = false;
        isFunctional = false;        
    }

    protected override void EnableFunctionality()
    {
        bodyPart.SetDefaultMaterial();
        basePart.SetDefaultMaterial();
        
        meshMouseNotifier.gameObject.SetActive(true);
        boxCollider.enabled = true;
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



    private void Shoot(Enemy enemyTarget)
    {     
        TurretAttack currentAttack = attackPool.GetObject().gameObject.GetComponent<TurretAttack>();
        currentAttack.transform.position = bodyPart.GetNextShootingPoint();
        currentAttack.transform.parent = attackPool.transform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.Init(enemyTarget, stats.damage);

        enemyTarget.QueueDamage(stats.damage);

        enemyTarget.ChangeMat();
    }


    private void AddEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeath += DeleteEnemyFromList;
        enemies.Add(enemy);
        enemies.Sort(mySort);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeath -= DeleteEnemyFromList;
        DeleteEnemyFromList(enemy);
    }

    private void DeleteEnemyFromList(Enemy enemyToDelete)
    {
        enemyToDelete.ChangeToBaseMat();
        enemies.Remove(enemyToDelete);

        enemies.Sort(mySort);
    }

    int mySort(Enemy e1, Enemy e2)
    {
        return e1.pathFollower.DistanceLeftToEnd.CompareTo(e2.pathFollower.DistanceLeftToEnd);
    }
}
