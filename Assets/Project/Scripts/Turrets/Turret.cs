using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

public class Turret : Building
{
    [Header("COMPONENTS")]
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Pool attackPool;

    [Header("STATS")]
    [SerializeField] private TurretStats stats;
    private float currentShootTimer;

    [Header("OTHERS")]
    [SerializeField] private Transform shootingPoint;

    private List<Enemy> enemies = new List<Enemy>();



    private void OnValidate()
    {
        boxCollider.size = new Vector3(stats.range, 1.0f, stats.range);
    }

    private void Awake()
    {
        currentShootTimer = 0.0f;
    }

    private void Update()
    {
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


    public override void GotPlaced(TurretStats turretStats)
    {
        InitStats(turretStats);
    }

    public void InitStats(TurretStats stats)
    {
        this.stats = stats;
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
