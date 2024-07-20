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


    private TurretStatsMultiplicationSnapshot _hyperStatsMultiplier = new TurretStatsMultiplicationSnapshot(3.0f, 0.25f, 2.0f);

    private const float HYPER_DAMAGE_MULTIPLIER = 3.0f;
    private const float HYPER_CADENCE_MULTIPLIER = 0.25f;
    private const float HYPER_RANGE_MULTIPLIER = 2.0f;

    private static bool _firstBerserkerPlaced = false;
    private const float BERSERK_START_DURATION = 2.0f;
    private const float BERSERK_DURATION_INCREMENT = 0.25f;
    private static float _berserkDuration;
    private float _berserkTimer;
    private Coroutine _berserkModeCoroutine;


    public static bool IsBerserkEnabled { get; private set; }


    public void TurretPlacedInit(TurretBuilding owner, Material material)
    {
        _owner = owner;
        _turretMaterial = new Material(material);
        owner.ResetBodyMaterial(_turretMaterial);

        _isBerserkEnabledProperty = Shader.PropertyToID("_IsBerserkEnabled");
        _berserkEyesOffsetProperty = Shader.PropertyToID("_BerserkEyesOffset");
        _berserkEyesScaleProperty = Shader.PropertyToID("_BerserkEyesScale");

        _flashEffectMaterial = _flashEffectMesh.material;
        _startTimeFlashProperty = Shader.PropertyToID("_StartTimeFlashAnimation");

        BerserkEyesData eyesData = _bodyTypeToEyesData[owner.BodyType];
        _turretMaterial.SetVector(_berserkEyesOffsetProperty, eyesData.offset);
        _turretMaterial.SetFloat(_berserkEyesScaleProperty, eyesData.scale);

        SubscribeToEvents();

        StopBerserkVisuals(false);


        if (!_firstBerserkerPlaced)
        {
            _firstBerserkerPlaced = true;
            _berserkDuration = BERSERK_START_DURATION;
        }
    }


    private void SubscribeToEvents()
    {
        _owner.OnDestroyed += UnsubscribeToEvents;
        _owner.OnBuildingUpgraded += CheckIsBerserkSetupHyperStats;

        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;
    }

    private void UnsubscribeToEvents()
    {
        _owner.OnDestroyed -= UnsubscribeToEvents;
        _owner.OnBuildingUpgraded -= CheckIsBerserkSetupHyperStats;

        PathLocation.OnTakeDamage -= OnPathLocationTakesDamage;

        _firstBerserkerPlaced = false;
    }



    private void OnPathLocationTakesDamage(PathLocation pathLocation)
    {
        EnterBerserkMode();
    }

    private void EnterBerserkMode()
    {
        _berserkTimer += _berserkDuration;

        if (!IsInBerserkMode())
        {
            _berserkModeCoroutine = StartCoroutine(BerserkMode());
        }
    }

    private IEnumerator BerserkMode()
    {
        SetupHyperStats();
        StartBerserkVisuals();
        IsBerserkEnabled = true;

        while (_berserkTimer > 0.0f)
        {
            _berserkTimer -= Time.deltaTime * GameTime.TimeScale;
            yield return null;
        }
        _berserkTimer = 0.0f;

        ResetStats();
        StopBerserkVisuals();
        IsBerserkEnabled = false;
        _berserkModeCoroutine = null;

        _berserkDuration += BERSERK_DURATION_INCREMENT;
    }

    private bool IsInBerserkMode()
    {
        return _berserkModeCoroutine != null;
    }

    private void CheckIsBerserkSetupHyperStats()
    {
        if (IsInBerserkMode())
        {
            SetupHyperStats();
        }
    }

    private void SetupHyperStats()
    {
        _owner.StatsBonusController.AddBonusStatsMultiplication(_hyperStatsMultiplier);
    }

    private void ResetStats()
    {
        _owner.StatsBonusController.RemoveBonusStatsMultiplication(_hyperStatsMultiplier);
    }


    private void StartBerserkVisuals()
    {
        _turretMaterial.SetFloat(_isBerserkEnabledProperty, 1.0f);
        _flashEffectMaterial.SetFloat(_startTimeFlashProperty, Time.time);

        GameAudioManager.GetInstance().PlayEnterBerserker();
    }
    private void StopBerserkVisuals(bool playAudio = true)
    {
        _turretMaterial.SetFloat(_isBerserkEnabledProperty, 0.0f);

        if (playAudio)
        {
            _flashEffectMaterial.SetFloat(_startTimeFlashProperty, Time.time - 0.5f);
            GameAudioManager.GetInstance().PlayExitBerserker();
        }
    }
}
