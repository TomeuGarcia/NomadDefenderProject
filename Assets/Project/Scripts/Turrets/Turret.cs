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
    Queue<int> enemies;

    [SerializeField] private Transform shootingPoint;

    //PROBLEMA AMB LA QUEUE
    //LA TORRE NO HA D'ATACAR A L'ULTIM ENEMIC QUE ENTRA, SI NO AL MÉS AVANÇAT EN EL CAMÍ

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemies.Enqueue(0);
            //enemies.Enqueue(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {

            //other.gameObject.GetComponent<Enemy>
        }
    }
}
