using DG.Tweening;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private EnemyAttackGeneralConfig _attackGeneralConfig;
    
    [Header("Mesh")]
    [SerializeField] private MeshRenderer meshRenderer;
    public Transform MeshTransform => meshRenderer.transform;
    private Vector3 originalMeshLocalScale;

    [Header("Components")]
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] public Transform transformToMove;
    [SerializeField] private Rigidbody rb;
    //[SerializeField] private BoxCollider boxCollider;
    [SerializeField] private HealthHUD healthHUD;
    [SerializeField] private EnemyFeedback enemyFeedback;
    //[SerializeField] private MeshRenderer armorCover;

    public PathFollower PathFollower => pathFollower;

    [Header("STATS")]
    [Expandable] [SerializeField] private EnemyTypeConfig _typeConfig;
    private int damage;
    private float armor;
    private float health;
    [HideInInspector] public int currencyDrop;

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
        

        if (armor == 0)
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
        damage = _typeConfig.BaseStats.Damage;
        health = _typeConfig.BaseStats.Health;
        armor = _typeConfig.BaseStats.Armor;
        currencyDrop = _typeConfig.BaseStats.CurrencyDrop;
        pathFollower.moveSpeed = _typeConfig.BaseStats.MoveSpeed;
    }

    public void SpawnedInit(PathNode startNode, Vector3 positionOffset, float totalDistance)
    {
        pathFollower.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), positionOffset, totalDistance);
    }


    private void TryAttackPathLocation(GameObject hitObject)
    {
        if (!collidedWithLocation)
        {
            if (hitObject.TryGetComponent<PathLocation>(out PathLocation pathLocation))
            {
                if (pathLocation.CanTakeDamage())
                {
                    pathLocation.TakeDamage(damage);
                    collidedWithLocation = true;

                    if (OnEnemyDeathDropCurrency != null) OnEnemyDeathDropCurrency(this);
                }
            }
        }

        Suicide();
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
        //Vector3 launchDirection = (transformToMove.forward + (transformToMove.up * 0.2f)).normalized;
        //rb.AddForce(launchDirection * 20f, ForceMode.Impulse);

        if (Physics.Raycast(Position, pathFollower.MoveDirection, out RaycastHit hit, 10, 
            _attackGeneralConfig.PathLocationAttackLayer, QueryTriggerInteraction.Collide))
        {
            TryAttackPathLocation(hit.collider.gameObject);
        }
    }

    public virtual int ComputeDamageWithPassive(TurretPartAttack_Prefab projectileSource, int damageAmount, PassiveDamageModifier modifier)
    {
        //Debug.Log("ComputeDamageWithPassive " + name);
        return modifier(damageAmount, healthSystem);
    }

    public void TakeDamage(TurretPartAttack_Prefab projectileSource, int damageAmount)
    {
        DoTakeDamage(projectileSource, damageAmount, out bool hitArmor);
    }
    
    public virtual void DoTakeDamage(TurretPartAttack_Prefab projectileSource, int damageAmount, out bool hitArmor)
    {
        healthHUD.Show();

        healthSystem.TakeDamage(damageAmount, out hitArmor);
        RemoveQueuedDamage(damageAmount);

        MeshTransform.localScale = originalMeshLocalScale;
        MeshTransform.DOKill(true);
        MeshTransform.DOPunchScale(originalMeshLocalScale * -0.3f, 0.2f, 4);

        if (healthSystem.IsDead())
        {
            Die();
        }

        SpawntakeDamageText(damageAmount, hitArmor);
    }

    private void SpawntakeDamageText(int damageAmount, bool hitArmor)
    {
        IFadingTextsFactory fadingTextsFactory = ServiceLocator.GetInstance().FadingTextFactory;
        IFadingTextsFactory.TextSpawnData textSpawnData = fadingTextsFactory.GetTextSpawnData();
        textSpawnData.Init(Position, damageAmount.ToString(), healthHUD.GetBarColor(hitArmor));
        fadingTextsFactory.SpawnFadingText(textSpawnData);
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
        gameObject.SetActive(false);
    }


    public virtual int QueueDamage(int amount)
    {
        queuedDamage += amount;
        return amount;
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
        health = (float)_typeConfig.BaseStats.Health * multiplier;

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
