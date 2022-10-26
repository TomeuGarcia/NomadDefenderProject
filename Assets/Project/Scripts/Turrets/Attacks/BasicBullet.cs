using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicProjectile : TurretAttack
{
    private Rigidbody2D rb;
    private float rotationSpeed;
    private float damage;

    private Transform target;

    [SerializeField] private float speed;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rotationSpeed = 10.0f;
    }

    override public void  Activate(Transform newTarget, float newDamage) 
    {
        damage = newDamage;
        target = newTarget;
    }

    private void Update()
    {
        Vector3 direction = (gameObject.transform.position - target.transform.position).normalized;
        rb.velocity = direction * speed;

        //IF ROTATION IS NEEDED
        //Quaternion lookRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject == target.gameObject)
        {
            //MAKE BULLET DEAL DAMAGE AND DISAPPEAR
            //target.gameObject.GetComponent<Enemy>().DealDamage(damage);
            StartCoroutine(Disappear());
        }
    }

    IEnumerator Disappear()
    {
        //PARTICLES
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
