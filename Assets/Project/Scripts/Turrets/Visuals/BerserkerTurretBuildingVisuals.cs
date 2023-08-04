using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BerserkerTurretBuildingVisuals : MonoBehaviour
{
    [SerializeField] private HurtedThresholdProjectile berserkerProjectilePrefab;

    private Material _material;
    private TurretBuilding _owner;
    private int _berserkHealthThreshold;
    private bool _isBerserk;

    private int _isBerserkEnabledProperty;
    private int _berserkEyesOffsetProperty;
    private int _berserkEyesScaleProperty;

    private static Dictionary<TurretPartBody.BodyType, BerserkEyesData> _bodyTypeToEyesData =
        new Dictionary<TurretPartBody.BodyType, BerserkEyesData>() {
            { TurretPartBody.BodyType.SENTRY, new BerserkEyesData(new Vector2(0.55f, -0.22f), 4.5f) },
            { TurretPartBody.BodyType.SPAMMER, new BerserkEyesData(new Vector2(0.55f, 0.2f), 2.0f) },
            { TurretPartBody.BodyType.BLASTER, new BerserkEyesData(new Vector2(0.4f, -0.05f), 2.5f) } 
        };


    private class BerserkEyesData
    {
        public BerserkEyesData(Vector2 offset, float scale)
        {
            this.offset = offset;
            this.scale = scale;
        }
        public Vector2 offset;
        public float scale;
    }


    public void TurretPlacedInit(TurretBuilding owner, Material material)
    {
        _owner = owner;
        _material = material;

        _berserkHealthThreshold = berserkerProjectilePrefab.HealthThreshold;

        _isBerserkEnabledProperty = Shader.PropertyToID("_IsBerserkEnabled");
        _berserkEyesOffsetProperty = Shader.PropertyToID("_BerserkEyesOffset");
        _berserkEyesScaleProperty = Shader.PropertyToID("_BerserkEyesScale");


        BerserkEyesData eyesData = _bodyTypeToEyesData[owner.BodyType];
        material.SetVector(_berserkEyesOffsetProperty, eyesData.offset);
        material.SetFloat(_berserkEyesScaleProperty, eyesData.scale);


        SubscribeToEvents();

        StopBerserk();
        CheckIsBerserk();
    }


    private void SubscribeToEvents()
    {
        _owner.OnDestroyed += UnsubscribeToEvents;

        PathLocation.OnHealthChanged += CheckIsBerserk;
    }

    private void UnsubscribeToEvents()
    {
        _owner.OnDestroyed -= UnsubscribeToEvents;

        PathLocation.OnHealthChanged -= CheckIsBerserk;
    }


    private void CheckIsBerserk()
    {
        int highestLocationHealth = ServiceLocator.GetInstance().TDLocationsUtils.GetHighestLocationHealth();
        bool berserkIsTriggered = highestLocationHealth <= _berserkHealthThreshold;
        Debug.Log(highestLocationHealth);

        if (!_isBerserk && berserkIsTriggered)
        {
            StartBerserk();
        }
        else if(_isBerserk && !berserkIsTriggered)
        {
            StopBerserk();
        }        
    }

    private void StartBerserk()
    {
        _isBerserk = true;

        _material.SetFloat(_isBerserkEnabledProperty, 1.0f);
    }
    private void StopBerserk()
    {
        _isBerserk = false;

        _material.SetFloat(_isBerserkEnabledProperty, 0.0f);
    }


}
