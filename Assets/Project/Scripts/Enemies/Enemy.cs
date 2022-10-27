using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Image healthImage;

    [Header("Stats")]
    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 2;


    private HealthSystem healthSystem;

    public delegate void OnDeathAction();

    public Vector3 Position => transformToMove.position;
    public Vector3 Right => transformToMove.right;


    private void Awake()
    {
        healthSystem = new HealthSystem(health);
    }

    private void OnEnable()
    {
        //TODO - RESET ENEMY

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
            Suicide();
            //Destroy(gameObject);//////////////////////////
        }
    }



    private void Attack()
    {
        Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        rb.AddForce(launchDirection * 20f, ForceMode.Impulse);
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);

        healthImage.fillAmount = healthSystem.healthRatio;

        if (healthSystem.IsDead())
        {
            Death();
        }
    }

    private void Suicide()
    {
        Deactivation();
    }

    private void Death()
    {
        Deactivation();
    }

    private void Deactivation()
    {
        gameObject.SetActive(false);
    }

    public float GetTravelDistance()
    {
        return pathFollower.GetTravelDistance();
    }
}
