using System;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour, ISpeedBoosterUser
{
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
    private int currencyDrop;

    // Queued damage
    private int queuedDamage = 0;

    public bool IsFakeEnemy { get; protected set; } = false;
    private bool collidedWithLocation = false;


    protected HealthSystem healthSystem;
    public HealthSystem HealthSystem => healthSystem;
    public EnemyAttackDestination AttackDestination { get; private set; }

    public delegate void EnemyAction(Enemy enemy);
    public static EnemyAction OnEnemySuicide;
    public static EnemyAction OnEnemyDeathGlobal;
    public EnemyAction OnEnemyDeath;
    public EnemyAction OnEnemyDeactivated;

    public Vector3 Position => meshRenderer.transform.position;
    public Vector3 Right => transformToMove.right;
    
    public bool CanBeTargetedFlag { get; set; }
    
    public EnemyWaveSpawner SpawnerOwner { get; private set; }
    

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
        pathFollower.OnPathEndReached += Attack;
    }

    private void OnDisable()
    {
        pathFollower.OnPathEndReached -= Attack;
        SpawnerOwner = null;
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
        pathFollower.UpdateBaseMoveSpeed(_typeConfig.BaseStats.MoveSpeed);
        pathFollower.SetMoveSpeedMultiplier(1f);

        CanBeTargetedFlag = true;
    }

    public void SpawnedInit(EnemyWaveSpawner spawner, PathNode startNode, float toNextNodeT, 
        Vector3 positionOffset, float totalDistance, EnemyAttackDestination attackDestination)
    {
        SpawnerOwner = spawner;
        ResetEnemy();
        AttackDestination = attackDestination;
        pathFollower.Init(startNode, positionOffset, totalDistance, toNextNodeT);
    }


    public virtual bool CanBeTargeted()
    {
        return CanBeTargetedFlag;
    }
    public virtual int GetTargetPriorityBonus()
    {
        return 0;
    }


    private void Attack()
    {
        PathLocation pathLocation = AttackDestination.GetLocationToAttack(pathFollower.CurrentTargetNode);

        if (pathLocation.CanTakeDamage())
        {
            pathLocation.TakeDamage(damage);
            collidedWithLocation = true;

            //ServiceLocator.GetInstance().CurrencySpawnService.SpawnCurrency(_typeConfig.BaseStats.CurrencyDrop, Position);
        }

        Suicide();
    }

    public virtual void OnWillBeAttacked(TurretDamageAttack damageAttack)
    {
        
    }
    

    public void TakeDamage(TurretDamageAttack damageAttack, Action<TurretDamageAttackResult> takeDamageResultCallback)
    {
        DoTakeDamage(damageAttack, takeDamageResultCallback);
    }

    protected virtual void DoTakeDamage(TurretDamageAttack damageAttack, Action<TurretDamageAttackResult> takeDamageResultCallback)
    {
        if (healthSystem.IsDead())
        {
            return;
        }
        
        healthHUD.Show();

        bool hadArmor = healthSystem.HasArmor();
        int previousHealth = healthSystem.health;
        int previousArmor = healthSystem.armor;
        
        healthSystem.TakeDamage(damageAttack.Damage, out bool hitArmor);
        
        bool brokeArmor = hadArmor && !healthSystem.HasArmor();
        int damageTaken = previousHealth - healthSystem.health;
        int armorDamageTaken = previousArmor - healthSystem.armor;
        
        
        RemoveQueuedDamage(damageAttack.Damage);

        MeshTransform.localScale = originalMeshLocalScale;
        MeshTransform.DOKill(true);
        MeshTransform.DOPunchScale(originalMeshLocalScale * -0.3f, 0.2f, 4);

        if (healthSystem.IsDead())
        {
            Die();
        }

        SpawntakeDamageText(damageAttack.Damage, hitArmor);
        AchievementDefinitions.OverkillDamage.Check(damageAttack.Damage);
        
        TurretDamageAttackResult result = 
            new TurretDamageAttackResult(damageAttack, this, damageTaken, armorDamageTaken, hitArmor, brokeArmor);
        
        takeDamageResultCallback(result);
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
        if (OnEnemyDeathGlobal != null) OnEnemyDeathGlobal(this);
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


    public virtual int QueueDamage(TurretDamageAttack damageAttack)
    {
        queuedDamage += damageAttack.Damage;
        return damageAttack.Damage;
    }

    public virtual void RemoveQueuedDamage(int amount) // use if enemy is ever healed
    {
        queuedDamage = (int)Mathf.Max(queuedDamage - amount, 0f);
    }

    public virtual bool DiesFromQueuedDamage()
    {
        return queuedDamage >= healthSystem.health;
    }

    public virtual void SetMoveSpeed(float speedCoef)
    {
        pathFollower.SetMoveSpeedMultiplier(speedCoef);
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

    public virtual bool CanBeAttackedByMultiCastProjectiles()
    {
        return true;
    }




    private Coroutine _speedBoostCoroutine = null;
    public void ApplySpeedBoosterMultiplier(SpeedBooster.Boost boost)
    {
        if (_speedBoostCoroutine != null)
        {
            StopCoroutine(_speedBoostCoroutine);
        }
        _speedBoostCoroutine = StartCoroutine(DoApplySpeedBoosterMultiplier(boost));
    }

    private IEnumerator DoApplySpeedBoosterMultiplier(SpeedBooster.Boost boost)
    {
        float boostedSpeed = _typeConfig.BaseStats.MoveSpeed * boost.SpeedMultiplier;
        
        Timer speedTransitionTimer = new Timer(boost.AccelerateDuration);
        while (!speedTransitionTimer.HasFinished())
        {
            speedTransitionTimer.Update(Time.deltaTime);
            pathFollower.UpdateBaseMoveSpeed(Mathf.LerpUnclamped(
                _typeConfig.BaseStats.MoveSpeed, boostedSpeed, speedTransitionTimer.Ratio01));
            
            yield return null;
        }
        pathFollower.UpdateBaseMoveSpeed(boost.SpeedMultiplier);

        
        yield return new WaitForSeconds(boost.Duration);
        
        
        speedTransitionTimer.Duration = boost.DecelerateDuration;
        while (!speedTransitionTimer.HasFinished())
        {
            speedTransitionTimer.Update(Time.deltaTime);
            pathFollower.UpdateBaseMoveSpeed(Mathf.LerpUnclamped(
                boostedSpeed, _typeConfig.BaseStats.MoveSpeed, speedTransitionTimer.Ratio01));
            
            yield return null;
        }
        pathFollower.UpdateBaseMoveSpeed(_typeConfig.BaseStats.MoveSpeed);

        _speedBoostCoroutine = null;
    }

    
}
