using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] int playCost;
    [SerializeField] int damage;
    [SerializeField] int shootCooldown;
    [SerializeField] int attackRange;
    [SerializeField] int targetAmount;
    [SerializeField] List<(int, float)> enemies = new List<(int, float)>();
    
    [SerializeField] private Transform shootingPoint;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            enemies.Add((1, (float)Random.Range(1, 10)));
            enemies.Sort(mySort);
            
            foreach ((int, float) enemy in enemies)
            {
                Debug.Log(enemy.Item1 + ", " + enemy.Item2);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            //Enemy currentEnemy = other.gameObject.GetComponent<Enemy>();
            //enemies.Enqueue(currentEnemy, currentEnemy.GetDistance());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemies.Sort(mySort);
            //Enemy currentEnemy = other.gameObject.GetComponent<Enemy>();
        }
    }

    int mySort((int, float) e1, (int, float) e2)
    {
        return e1.Item2.CompareTo(e2.Item2);
    }
}
