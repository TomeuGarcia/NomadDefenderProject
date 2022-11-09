using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PathFollower;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { BASIC, FAST, TANK }
    
    [Header("Mesh")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material selectedMaterial;

    [Header("Components")]
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private HealthHUD healthHUD;

    [Header("Stats")]
    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 2;
    [SerializeField] public int currencyDrop;

    // Queued damage
    private int queuedDamage = 0;   



    private HealthSystem healthSystem;

    public delegate void EnemyAction(Enemy enemy);
    public static EnemyAction OnEnemyDeathDropCurrency;
    public EnemyAction OnEnemyDeactivated;

    public Vector3 Position => meshRenderer.transform.position;
    public Vector3 Right => transformToMove.right;

    private void Awake()
    {
        healthSystem = new HealthSystem(health);
        healthHUD.Init(healthSystem);
    }

    private void OnValidate()
    {
        boxCollider.center = meshRenderer.gameObject.transform.localPosition;
        boxCollider.size = meshRenderer.gameObject.transform.localScale;
    }

    private void OnEnable()
    {
        ResetEnemy();

        pathFollower.OnPathEndReached += Attack;
    }

    private void OnDisable()
    {
        pathFollower.OnPathEndReached -= Attack;
    }

    private void ResetEnemy()
    {
        StopAllCoroutines();

        meshRenderer.material = baseMaterial;
        healthSystem.HealToMax();

        queuedDamage = 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathLocation"))
        {
            other.gameObject.GetComponent<PathLocation>().TakeDamage(damage);
            Suicide();
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
        RemoveQueuedDamage(damageAmount);

        if (healthSystem.IsDead())
        {
            Die();
        }
    }

    private void Suicide()
    {
        Deactivation();
    }

    private void Die()
    {
        if (OnEnemyDeathDropCurrency != null) OnEnemyDeathDropCurrency(this);
        Deactivation();
    }

    private void Deactivation()
    {
        if (OnEnemyDeactivated != null) OnEnemyDeactivated(this);

        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void ChangeMat()
    {
        meshRenderer.material = selectedMaterial;
    }
    public void ChangeToBaseMat()
    {
        meshRenderer.material = baseMaterial;
    }


    public void QueueDamage(int amount)
    {
        queuedDamage += amount;
    }

    private void RemoveQueuedDamage(int amount) // use if enemy is ever healed
    {
        queuedDamage -= amount;
    }

    public bool DiesFromQueuedDamage()
    {
        return queuedDamage >= healthSystem.health;
    }

    public void SetMoveSpeed(float speedCoef)
    {
        pathFollower.SetMoveSpeed(speedCoef);
    }
}
