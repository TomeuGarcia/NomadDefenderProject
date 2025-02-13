using System;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour, ISpeedBoosterUser
{
    [Header("Mesh")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Transform _meshHolder;
    [SerializeField] private Transform _meshCenter;
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
    public int Damage { get; private set; }
    private int _armor;
    private int _health;
    private int currencyDrop;

    public EnemyTypeConfig TypeConfig => _typeConfig;
    
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
    public EnemyAction OnBeforeEnemyDeath;
    public EnemyAction OnEnemyDeath;
    public EnemyAction OnEnemyDeactivated;

    public Vector3 Position => meshRenderer.transform.position;
    public Vector3 Right => transformToMove.right;
    
    public bool CanBeTargetedFlag { get; set; }
    
    public EnemyWaveSpawner SpawnerOwner { get; private set; }

    public static Action<EnemyTypeConfig, TurretDamageAttack> OnTakeDamage;
    public static Action<TurretDamageAttackResult> OnTakeDamageResult;
    public static Action<EnemyTypeConfig, int> OnDealDamage;

    public static Action<Enemy, PathLocation> OnTriedToAttackDeadLocation;

    private bool _initializedWithoutFunctionality;

    private static readonly Quaternion _particleSpawnRotation = Quaternion.Euler(90, 0, 0);
    
    
    private void Awake()
    {
        ResetStats();
        

        if (_armor == 0)
        {
            healthSystem = new HealthSystem(_health);
        }
        else
        {
            healthSystem = new HealthSystem(_health, _armor);
        }

        healthHUD.Init(healthSystem);

        originalMeshLocalScale = _meshHolder.localScale;

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

    public void InitWithoutFunctionality()
    {
        ResetEnemy();
        healthHUD.gameObject.SetActive(false);
        _initializedWithoutFunctionality = true;
        
        enabled = false;
        pathFollower.enabled = false;

        if (_meshHolder.TryGetComponent(out EnemyIdleAnimator idleAnimator))
        {
            idleAnimator.enabled = false;
        }
        if (TryGetComponent(out AreaSpawnerArmor areaSpawnerArmor))
        {
            areaSpawnerArmor.enabled = false;
        }
        if (TryGetComponent(out AreaSpawnerHealth areaSpawnerHealth))
        {
            areaSpawnerHealth.enabled = false;
        }
    }

    public void PositionWithCenteredMesh()
    {
        Vector3 desiredCenterPosition = transform.position;
        Vector3 currentCenterPosition = _meshCenter.position;
        Vector3 offset = currentCenterPosition - desiredCenterPosition;
        transform.position = desiredCenterPosition - offset;

        if (_meshHolder.TryGetComponent(out EnemyIdleAnimator idleAnimator))
        {
            idleAnimator.Stop();
        }
    }

    private void ResetStats()
    {
        Damage = _typeConfig.BaseStats.Damage;
        _health = _typeConfig.BaseStats.Health;
        _armor = _typeConfig.BaseStats.Armor;
        currencyDrop = _typeConfig.BaseStats.CurrencyDrop;
        pathFollower.UpdateBaseMoveSpeed(_typeConfig.BaseStats.MoveSpeed);
        pathFollower.SetMoveSpeedMultiplier(1f);
        _ignoreStunned = false;

        CanBeTargetedFlag = true;
    }

    public void SpawnedInit(EnemyWaveSpawner spawner, PathNode startNode, float toNextNodeT, 
        Vector3 positionOffset, float totalDistance, EnemyAttackDestination attackDestination)
    {
        SpawnerOwner = spawner;
        ResetEnemy();
        AttackDestination = attackDestination;
        pathFollower.Init(startNode, positionOffset, totalDistance, toNextNodeT);

        _initializedWithoutFunctionality = false;

        ServiceLocator.GetInstance().ParticleFactory
            .Create(_typeConfig.View.ParticlesSpawn, _meshCenter.position, _particleSpawnRotation);
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
        if (!AttackPathLocation(pathLocation))
        {
            OnTriedToAttackDeadLocation?.Invoke(this, pathLocation);
        }
        
        ServiceLocator.GetInstance().ParticleFactory
            .Create(_typeConfig.View.ParticlesAttack, Position, Quaternion.identity);
        
        Suicide();
    }

    public bool AttackPathLocation(PathLocation pathLocation)
    {
        if (pathLocation.CanTakeDamage())
        {
            pathLocation.TakeDamage(Damage);
            collidedWithLocation = true;

            //ServiceLocator.GetInstance().CurrencySpawnService.SpawnCurrency(_typeConfig.BaseStats.CurrencyDrop, Position);
            OnDealDamage?.Invoke(_typeConfig, Damage);
            return true;
        }

        return false;
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

        _meshHolder.localScale = originalMeshLocalScale;
        _meshHolder.DOKill(true);
        _meshHolder.DOPunchScale(originalMeshLocalScale * -0.3f, 0.2f, 4);

        bool gotKilled = healthSystem.IsDead();
        if (gotKilled && !_initializedWithoutFunctionality)
        {
            Die();
        }

        OnTakeDamage?.Invoke(_typeConfig, damageAttack);
        SpawntakeDamageText(damageAttack, hitArmor);
        AchievementDefinitions.OverkillDamage.Check(damageAttack.Damage);
        
        TurretDamageAttackResult result = 
            new TurretDamageAttackResult(damageAttack, this, damageTaken, armorDamageTaken, hitArmor, brokeArmor, gotKilled);
        
        OnTakeDamageResult?.Invoke(result);
        
        takeDamageResultCallback(result);
    }

    private void SpawntakeDamageText(TurretDamageAttack damageAttack, bool hitArmor)
    {
        IFadingTextsFactory fadingTextsFactory = ServiceLocator.GetInstance().FadingTextFactory;
        IFadingTextsFactory.TextSpawnData textSpawnData = fadingTextsFactory.GetTextSpawnData();

        Color textColor = damageAttack.ProjectileSource != null
            ? damageAttack.ProjectileSource.TurretOwner.ProjectileDataModel.materialColor
            : healthHUD.GetBarColor(hitArmor);
        textSpawnData.Init(Position, damageAttack.Damage.ToString(), textColor);
        //textSpawnData.Init(Position, damageAmount.ToString(), healthHUD.GetBarColor(hitArmor));
        
        fadingTextsFactory.SpawnFadingText(textSpawnData);
    }



    public virtual void GetStunned(float duration)
    {
        if (IsDead() || _ignoreStunned) return;
        pathFollower.PauseForDuration(duration);
        StartCoroutine(DoIgnoreStunned(duration));
    }

    private bool _ignoreStunned = false;

    private IEnumerator DoIgnoreStunned(float stunDuration)
    {
        _ignoreStunned = true;
        yield return new WaitForSeconds(stunDuration + 0.2f);
        _ignoreStunned = false;
    }

    private void Suicide()
    {
        if (OnEnemySuicide != null) OnEnemySuicide(this);
        Deactivation();
    }

    private void Die()
    {
        OnBeforeEnemyDeath?.Invoke(this);
        if (OnEnemyDeathGlobal != null) OnEnemyDeathGlobal(this);
        if (OnEnemyDeath != null) OnEnemyDeath(this);
        
        ServiceLocator.GetInstance().ParticleFactory
            .Create(_typeConfig.View.ParticlesDeath, Position, Quaternion.identity);
        
        Deactivation();
    }

    protected void Deactivation()
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
        _health = Mathf.RoundToInt(_typeConfig.BaseStats.Health * multiplier);

        healthSystem.UpdateHealth(_health);
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
