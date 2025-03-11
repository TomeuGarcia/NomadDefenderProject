using System;
using NaughtyAttributes;
using Scripts.ObjectPooling;
using UnityEngine;

public class ElectricWireSegment : RecyclableObject
{
    public interface IAttachable
    {
        Vector3 GetAttachPosition();
    }
    public interface IEnemyDamageData
    {
        int GetDamage();
        float GetStunDuration();
        Gradient GetColorOverLifetimeGradient();
        int GetStackedDamage();
        float GetStackedStunDuration();
        Gradient GetStackedColorOverLifetimeGradient();
        
    }

    
    
    [System.Serializable]
    private class Vertex
    {
        [SerializeField] private Transform _holder;
        [SerializeField] private ParticleSystem _lightningParticles;
        [SerializeField] private ParticleSystem _sparksParticles;


        public void Place(Vector3 origin, float originToTargetDistance, Quaternion originToTargetRotation)
        {
            _holder.position = origin;
            _holder.rotation = originToTargetRotation;

            
            ParticleSystem.VelocityOverLifetimeModule lightningVelocityOverLifetimeModule =
                _lightningParticles.velocityOverLifetime;
            lightningVelocityOverLifetimeModule.orbitalOffsetZ = originToTargetDistance;

            
            ParticleSystem.MainModule lightningMainModule = _lightningParticles.main;
            lightningMainModule.startLifetime = originToTargetDistance / 10f;
            
            ParticleSystem.MainModule sparksMainModule = _sparksParticles.main;
            sparksMainModule.startLifetime = (originToTargetDistance / 10f) * 2f;
            
            _lightningParticles.Play();
            _sparksParticles.Play();
        }

        public void SetColor(Gradient colorOverLifetimeGradient)
        {
            ParticleSystem.ColorOverLifetimeModule lightningColorOverLifetimeModule =
                _lightningParticles.colorOverLifetime;
            lightningColorOverLifetimeModule.color = colorOverLifetimeGradient;
            
            /*
            ParticleSystem.ColorOverLifetimeModule sparksColorOverLifetimeModule =
                _sparksParticles.colorOverLifetime;
            sparksColorOverLifetimeModule.color = colorOverLifetimeGradient;
            */
        }
    }


    [SerializeField] private BoxCollider _damageTrigger;
    [SerializeField] private TriggerNotifier _triggerNotifier;
    [SerializeField] private Vertex _vertexA;
    [SerializeField] private Vertex _vertexB;

    private IAttachable[] _owners;
    private IAttachable _otherAttachable;
    private IEnemyDamageData _enemyDamageData;
    
    private int _damage;
    private float _stunDuration;
    



    public void SetEnemyDamageData(IEnemyDamageData enemyDamageData)
    {
        _enemyDamageData = enemyDamageData;
    }

    public void SetSingleOwner(IAttachable owner, IAttachable otherAttachable)
    {
        _owners = new[] { owner };
        _otherAttachable = otherAttachable;

        _damage = _enemyDamageData.GetDamage();
        _stunDuration = _enemyDamageData.GetStunDuration();
        
        UpdateColor(false);
    }
    public void SetMultipleOwners(IAttachable ownerA, IAttachable ownerB)
    {
        _owners = new[] { ownerA, ownerB };
        _otherAttachable = null;
        
        _damage = _enemyDamageData.GetStackedDamage();
        _stunDuration = _enemyDamageData.GetStackedStunDuration();
        
        UpdateColor(true);
    }

    public bool HasOwner(IAttachable possibleOwner)
    {
        foreach (IAttachable owner in _owners)
        {
            if (owner == possibleOwner)
            {
                return true;
            }
        }

        return false;
    }
    public bool HasOther(IAttachable possibleOtherAttachable)
    {
        foreach (IAttachable owner in _owners)
        {
            if (owner == possibleOtherAttachable)
            {
                return true;
            }
        }

        return _otherAttachable == possibleOtherAttachable;
    }
    
    
    

    public void SetPlacement(Vector3 positionA, Vector3 positionB)
    {
        PlaceVertices(positionA, positionB);
    }
    
    
    private void PlaceVertices(Vector3 positionA, Vector3 positionB)
    {
        Vector3 originToTarget = positionB - positionA;
        float originToTargetDistance = originToTarget.magnitude;
        Vector3 originToTargetDirection = originToTarget / originToTargetDistance;
            
        Quaternion originToTargetRotation = Quaternion.LookRotation(originToTargetDirection, Vector3.up);
        Quaternion targetToOriginRotation = Quaternion.LookRotation(-originToTargetDirection, Vector3.up);
        
        
        _vertexA.Place(positionA, originToTargetDistance, originToTargetRotation);
        _vertexB.Place(positionB, originToTargetDistance, targetToOriginRotation);

        _damageTrigger.transform.position = Vector3.LerpUnclamped(positionA, positionB, 0.5f);
        _damageTrigger.transform.rotation = originToTargetRotation;
        _damageTrigger.size = new Vector3(0.25f, 1.0f, originToTargetDistance);
    }

    private void UpdateColor(bool isStacked)
    {
        Gradient colorOverLifetimeGradient = isStacked
            ? _enemyDamageData.GetStackedColorOverLifetimeGradient()
            : _enemyDamageData.GetColorOverLifetimeGradient();
        
        _vertexA.SetColor(colorOverLifetimeGradient);
        _vertexB.SetColor(colorOverLifetimeGradient);
    }
    


    public void DoGetRemoved()
    {
        Recycle();
    }

    internal override void RecycledInit()
    {
        _triggerNotifier.OnEnter += OnObjectTriggerEnter;
        _damageTrigger.enabled = true;
    }

    internal override void RecycledReleased()
    {
        _triggerNotifier.OnEnter -= OnObjectTriggerEnter;
        _damageTrigger.enabled = false;
        for (int i = 0; i < _owners.Length; ++i)
        {
            _owners[i] = null;
        }
        _otherAttachable = null;
    }


    private void OnObjectTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            ApplyDamageToEnemy(enemy);
        }
    }

    private void ApplyDamageToEnemy(Enemy enemy)
    {
        TurretDamageAttack damageAttack = new TurretDamageAttack(null, enemy, _damage, false);
        enemy.TakeDamage(damageAttack, OnAfterEnemyReceivingDamage);
        enemy.GetStunned(_stunDuration);
    }

    private void OnAfterEnemyReceivingDamage(TurretDamageAttackResult damageAttackResult)
    {
        ProjectileParticleFactory.GetInstance().CreateParticlesGameObject(ProjectileParticleType.StunTouch_Hit,
            damageAttackResult.Target.Position, Quaternion.identity);
    }
}