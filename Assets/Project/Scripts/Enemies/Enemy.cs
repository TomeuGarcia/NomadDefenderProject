using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PathFollower;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { BASIC, FAST, TANK,BASIC_ARMORED,FAST_ARMORED,TANK_ARMORED, ARMOR_TRUCK,HEALTH_TRUCK}
    
    [Header("Mesh")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material selectedMaterial;
    public Transform MeshTransform => meshRenderer.transform;
    private Vector3 originalMeshLocalScale;

    [Header("Components")]
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] public Rigidbody rb;
    //[SerializeField] private BoxCollider boxCollider;
    [SerializeField] private HealthHUD healthHUD;
    [SerializeField] private EnemyFeedback enemyFeedback;
    //[SerializeField] private MeshRenderer armorCover;

    [Header("Stats")]
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private int baseHealth = 2;
    [SerializeField] private int baseArmor = 0;
    private float damage;
    private float armor;
    private float health;
    [HideInInspector] public int currencyDrop;
    [SerializeField] public int baseCurrencyDrop;

    // Queued damage
    private int queuedDamage = 0;   



    private HealthSystem healthSystem;

    public delegate void EnemyAction(Enemy enemy);
    public static EnemyAction OnEnemyDeathDropCurrency;
    public EnemyAction OnEnemyDeath;
    public EnemyAction OnEnemyDeactivated;

    public Vector3 Position => meshRenderer.transform.position;
    public Vector3 Right => transformToMove.right;

    private void Awake()
    {
        ResetStats();

        if(armor == 0)
        {
            healthSystem = new HealthSystem((int)health);
        }
        else
        {
            healthSystem = new HealthSystem((int)health, (int)armor);
        }

        healthHUD.Init(healthSystem);

        originalMeshLocalScale = MeshTransform.localScale;

        healthSystem.OnArmorUpdated += enemyFeedback.ArmorUpdate;
    }

    private void OnValidate()
    {
        //boxCollider.center = meshRenderer.gameObject.transform.localPosition;
        //boxCollider.size = meshRenderer.gameObject.transform.localScale;
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

        //ChangeToBaseMat();
        healthSystem.HealToMax();
        healthSystem.ResetArmor();

        queuedDamage = 0;

        ResetStats();

        healthHUD.Hide();

        enemyFeedback.ResetEnemy(healthSystem.HasArmor());
    }

    private void ResetStats()
    {
        damage = baseDamage;
        health = baseHealth;
        armor = baseArmor;

        currencyDrop = baseCurrencyDrop;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathLocation"))
        {
            PathLocation pathLocation = other.gameObject.GetComponent<PathLocation>();
            if (!pathLocation.IsDead)
            {
                pathLocation.TakeDamage((int)damage);
            }
            Suicide();
        }
    }



    private void Attack()
    {
        Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        rb.AddForce(launchDirection * 20f, ForceMode.Impulse);
    }

    public void TakeDamage(int damageAmount, PassiveDamageModifier modifier)
    {
        healthHUD.Show();

        damageAmount = modifier(damageAmount, healthSystem);
        healthSystem.TakeDamage(damageAmount);
        RemoveQueuedDamage(damageAmount);

        MeshTransform.localScale = originalMeshLocalScale;
        MeshTransform.DOKill(true);
        MeshTransform.DOPunchScale(originalMeshLocalScale * -0.3f, 0.2f, 4);

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
        if (OnEnemyDeath != null) OnEnemyDeath(this);
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
        //meshRenderer.material = selectedMaterial;
    }
    public void ChangeToBaseMat()
    {
        //meshRenderer.material = baseMaterial;
    }


    public int QueueDamage(int amount, PassiveDamageModifier modifier)
    {
        amount = modifier(amount, healthSystem);
        queuedDamage += amount;

        //if (queuedDamage >= healthSystem.health)
        //{
        //    StartCoroutine(TimedDeath());
        //}

        return amount;
    }

    IEnumerator TimedDeath()
    {
        Debug.LogWarning("Enemy Death for timer");
        yield return new WaitForSeconds(0.5f);
        Die();
    }

    public void RemoveQueuedDamage(int amount) // use if enemy is ever healed
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

    public void ApplyWaveStatMultiplier(float multiplier)
    {
        damage = (float)baseDamage * multiplier;
        health = (float)baseHealth * multiplier;

        healthSystem.UpdateHealth((int)health);
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public void AddHealth(int healthToAdd)
    {
        
        healthSystem.Heal(healthToAdd);
        healthHUD.Show();

    }

    public void AddArmor(int armorToAdd)
    {
        
        healthSystem.AddArmor(armorToAdd);
        healthHUD.Show();
    }
}
