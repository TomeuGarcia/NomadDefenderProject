using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : Building
{
    private BoxCollider bc;
    private Pool attackPool;

    [Header("STATS")]
    [SerializeField] int playCost;
    [SerializeField] int damage;
    [SerializeField] int attackRange;
    [SerializeField] int targetAmount;
    [SerializeField] float shootCooldown;
    private float currentShootTimer;

    [Header("OTHERS")]
    [SerializeField] private Transform shootingPoint;
    
    private List<(Enemy, float)> enemies = new List<(Enemy, float)>();

    private void Awake()
    {
        attackPool = GameObject.Find("BasicTurretAttackPool").gameObject.GetComponent<Pool>();
        currentShootTimer = 0.0f;

        bc = gameObject.GetComponent<BoxCollider>();
        bc.size = new Vector3(attackRange, 1.0f, attackRange);
    }

    private void Update()
    {
        if(currentShootTimer < shootCooldown)
        {
            currentShootTimer += Time.deltaTime;
        }
        else
        {
            if (enemies.Count > 0)
            {
                currentShootTimer = 0.0f;

                for (int i = 0; i < targetAmount; i++)
                {
                    if(i <= enemies.Count - 1)
                    {
                        Shoot(enemies[i].Item1);
                    }
                }
            }
        }
    }

    private void Shoot(Enemy enemyTarget)
    {
        TurretAttack currentAttack = attackPool.GetObject().gameObject.GetComponent<TurretAttack>();
        currentAttack.transform.position = shootingPoint.position;
        currentAttack.gameObject.SetActive(true);
        currentAttack.Init(enemyTarget, damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemies.Add((enemy, enemy.GetTravelDistance()));
            enemies.Sort(mySort);
        }
    }

    private void OnTriggerExit(Collider other)  //NOT WORKING
    {
        if (other.tag == "Enemy")
        {
            int i = 0;
            foreach((Enemy, float) enemy in enemies)
            {
                if(enemy.Item1 == other.gameObject.GetComponent<Enemy>())
                {
                    enemies.RemoveAt(i);
                    break;
                }
                i++;
            }

            enemies.Sort(mySort);
        }
    }

    int mySort((Enemy, float) e1, (Enemy, float) e2)
    {
        return e1.Item2.CompareTo(e2.Item2);
    }
}
