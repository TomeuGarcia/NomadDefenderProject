using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BerserkerTurretBuildingVisuals : MonoBehaviour
{
    [SerializeField] private HurtedThresholdProjectile _berserkerProjectilePrefab;
    [SerializeField] private MeshRenderer _flashEffectMesh;
    private Material _flashEffectMaterial;

    private Material _turretMaterial;
    private TurretBuilding _owner;
    private int _berserkHealthThreshold;
    private bool _isBerserk;

    private int _isBerserkEnabledProperty;
    private int _berserkEyesOffsetProperty;
    private int _berserkEyesScaleProperty;

    private int _startTimeFlashProperty;

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
        _turretMaterial = new Material(material);
        owner.ResetBodyMaterial(_turretMaterial);

        _berserkHealthThreshold = _berserkerProjectilePrefab.HealthThreshold;

        _isBerserkEnabledProperty = Shader.PropertyToID("_IsBerserkEnabled");
        _berserkEyesOffsetProperty = Shader.PropertyToID("_BerserkEyesOffset");
        _berserkEyesScaleProperty = Shader.PropertyToID("_BerserkEyesScale");

        _flashEffectMaterial = _flashEffectMesh.material;
        _startTimeFlashProperty = Shader.PropertyToID("_StartTimeFlashAnimation");

        BerserkEyesData eyesData = _bodyTypeToEyesData[owner.BodyType];
        _turretMaterial.SetVector(_berserkEyesOffsetProperty, eyesData.offset);
        _turretMaterial.SetFloat(_berserkEyesScaleProperty, eyesData.scale);

        SubscribeToEvents();

        StopBerserk(false);
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

        _turretMaterial.SetFloat(_isBerserkEnabledProperty, 1.0f);
        _flashEffectMaterial.SetFloat(_startTimeFlashProperty, Time.time);

        GameAudioManager.GetInstance().PlayEnterBerserker();
    }
    private void StopBerserk(bool playAudio = true)
    {
        _isBerserk = false;

        _turretMaterial.SetFloat(_isBerserkEnabledProperty, 0.0f);

        if (playAudio)
        {
            GameAudioManager.GetInstance().PlayExitBerserker();
        }
    }


}
