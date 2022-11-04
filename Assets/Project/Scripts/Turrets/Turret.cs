using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] private GameObject rangePlaneMeshObject;
    private Material rangePlaneMaterial;

    [Header("COMPONENTS")]
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Pool attackPool;
    [SerializeField] private MouseOverNotifier meshMouseNotifier;

    [Header("STATS")]
    private TurretStats stats;
    private float currentShootTimer;

    [Header("OTHERS")]
    [SerializeField] private Transform shootingPoint;

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
        if (!isFunctional) return;

        if(currentShootTimer < stats.cadence)
        {
            currentShootTimer += Time.deltaTime;
        }
        else
        {
            if (enemies.Count > 0)
            {
                currentShootTimer = 0.0f;

                for (int i = 0; i < stats.targetAmount; i++)
                {
                    if(i <= enemies.Count - 1)
                    {
                        Shoot(enemies[i]);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnEnemyDeath += DeleteEnemyFromList;
            enemies.Add(enemy);
            enemies.Sort(mySort);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().OnEnemyDeath -= DeleteEnemyFromList;
            DeleteEnemyFromList(other.gameObject.GetComponent<Enemy>());
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
        currentAttack.Init(enemyTarget, stats.damage);

        enemyTarget.ChangeMat();
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
