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
    public enum EnemyType { 
        BASIC, // 0
        FAST,  // 1
        TANK,BASIC_ARMORED, // 2
        FAST_ARMORED,TANK_ARMORED, // 3
        ARMOR_TRUCK,  // 4
        HEALTH_TRUCK, // 5
        COUNT
    }
    
    [Header("Mesh")]
    [SerializeField] private MeshRenderer meshRenderer;
    public Transform MeshTransform => meshRenderer.transform;
    private Vector3 originalMeshLocalScale;

    [Header("Components")]
    [SerializeField] public PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] private Rigidbody rb;
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

    public bool IsFakeEnemy { get; protected set; } = false;
    private bool collidedWithLocation = false;


    protected HealthSystem healthSystem;

    public delegate void EnemyAction(Enemy enemy);
    public static EnemyAction OnEnemyDeathDropCurrency;
    public static EnemyAction OnEnemySuicide;
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
        IsFakeEnemy = false;
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

        collidedWithLocation = false;

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
        if (other.CompareTag("PathLocation") && !collidedWithLocation)
        {
            Debug.Log(this.name);

            PathLocation pathLocation = other.gameObject.GetComponent<PathLocation>();
            if (pathLocation.CanTakeDamage())
            {
                //pathLocation.TakeDamage((int)damage);
                pathLocation.TakeDamage(1);
                collidedWithLocation = true;
            }
            Suicide();
        }
    }


    public virtual bool CanBeTargeted()
    {
        return true;
    }
    public virtual float GetTargetNegativePriorityBonus()
    {
        return 0f;
    }


    private void Attack()
    {
        Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        rb.AddForce(launchDirection * 20f, ForceMode.Impulse);
    }

    public virtual int ComputeDamageWithPassive(TurretPartAttack_Prefab projectileSource, int damageAmount, PassiveDamageModifier modifier)
    {
        //Debug.Log("ComputeDamageWithPassive " + name);
        return modifier(damageAmount, healthSystem);
    }

    public virtual void TakeDamage(TurretPartAttack_Prefab projectileSource, int damageAmount)
    {
        healthHUD.Show();

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

    public virtual void GetStunned(float duration)
    {
        if (IsDead()) return;
        pathFollower.PauseForDuration(duration);
    }

    private void Suicide()
    {
        if (OnEnemySuicide != null) OnEnemySuicide(this);
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

        enemyFeedback.FinishCoroutines();

        pathFollower.CheckDeactivateCoroutines();
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }


    public virtual int QueueDamage(int amount)
    {
        queuedDamage += amount;
        return amount;
    }

    IEnumerator TimedDeath()
    {
        Debug.LogWarning("Enemy Death for timer");
        yield return new WaitForSeconds(0.5f);
        Die();
    }

    public virtual void RemoveQueuedDamage(int amount) // use if enemy is ever healed
    {
        queuedDamage -= amount;
    }

    public virtual bool DiesFromQueuedDamage()
    {
        return queuedDamage >= healthSystem.health;
    }

    public virtual void SetMoveSpeed(float speedCoef)
    {
        pathFollower.SetMoveSpeed(speedCoef);
    }

    public virtual void ApplyWaveStatMultiplier(float multiplier)
    {
        damage = (float)baseDamage * multiplier;
        health = (float)baseHealth * multiplier;

        healthSystem.UpdateHealth((int)health);
    }

    public virtual bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public virtual void AddHealth(int healthToAdd)
    {
        
        healthSystem.Heal(healthToAdd);
        healthHUD.Show();

    }

    public virtual void AddArmor(int armorToAdd)
    {
        
        healthSystem.AddArmor(armorToAdd);
        healthHUD.Show();
    }


    public virtual Vector3 GetPosition()
    {
        return Position;
    }
}
