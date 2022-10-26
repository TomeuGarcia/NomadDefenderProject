using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private RectTransform canvasTransform;

    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 2;
    private HealthSystem healthSystem;


    public Vector3 Position => transformToMove.position;
    public Vector3 Right => transformToMove.right;


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
    }

    private void OnEnable()
    {
        pathFollower.OnPathEndReached += Attack;
    }

    private void OnDisable()
    {
        pathFollower.OnPathEndReached -= Attack;
    }

    private void Update()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        //cameraDirection.y = 0;

        canvasTransform.rotation = Quaternion.LookRotation(cameraDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathLocation"))
        {
            other.gameObject.GetComponent<PathLocation>().TakeDamage(damage);
            Destroy(gameObject); ///////////////////////////////////////////////////////////
        }
    }



    private void Attack()
    {
        Debug.Log("Attack");

        Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        rb.AddForce(launchDirection * 20f, ForceMode.Impulse);
    }


}
