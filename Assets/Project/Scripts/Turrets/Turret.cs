using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] private GameObject rangePlaneMeshObject;
    private Material rangePlaneMaterial;

    [Header("COMPONENTS")]
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Pool attackPool;
    [SerializeField] private MouseOverNotifier meshMouseNotifier;

    [Header("OTHERS")]
    [SerializeField] private Transform shootingPoint;

    // STATS
    public TurretStats stats { get; private set; }
    private float currentShootTimer;


    private List<Enemy> enemies = new List<Enemy>();

    private bool isFunctional = false;



    private void OnValidate()
    {
        boxCollider.size = new Vector3(stats.range, 1.0f, stats.range);
        rangePlaneMeshObject.transform.localScale = Vector3.one * (stats.range / 10f);
    }

    private void Awake()
    {
        currentShootTimer = 0.0f;
        DisableFunctionality();
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


    public override void Init(TurretStats turretStats)
    {
        InitStats(turretStats);

        boxCollider.size = new Vector3(stats.range, 1.0f, stats.range);
        rangePlaneMeshObject.transform.localScale = Vector3.one * (stats.range / 10f);
        rangePlaneMaterial = rangePlaneMeshObject.GetComponent<MeshRenderer>().materials[0];
        rangePlaneMaterial.SetFloat("_TileNum", (float)stats.range);
        rangePlaneMaterial.SetColor("_Color", Color.cyan);

        //Color[] colors = new Color[] { Color.cyan, Color.yellow, Color.green, Color.red, Color.blue }; // Debug purposes
        //rangePlaneMaterial.SetColor("_Color", colors[Random.Range(0, colors.Length)]);
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
        base.DisableFunctionality();
        boxCollider.enabled = false;
        isFunctional = false;
    }

    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();
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
        currentAttack.transform.position = shootingPoint.position;
        currentAttack.transform.parent = attackPool.transform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.Init(enemyTarget, this);

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



    /// <summary>
    /// Returns a List with size up to "amount" 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public Enemy[] GetEnemies(int amount)
    {
        if (amount > enemies.Count)
            amount = enemies.Count;

        Enemy[] enemyArray = new Enemy[amount];

        for (int i = 0; i < amount; ++i)
        {
            enemyArray[i] = enemies[i];
        }

        return enemyArray;
    }


}
