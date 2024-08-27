using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerTurretBuildingVisuals : MonoBehaviour
{
    [SerializeField] private MeshRenderer _flashEffectMesh;
    private Material _flashEffectMaterial;

    private Material _turretMaterial;

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
    

    //public static bool IsBerserkEnabled { get; private set; }


    public void TurretPlacedInit(TurretBuilding owner, Material material)
    {
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
        
        StopBerserkVisuals(false);
    }


    public void StartBerserkVisuals()
    {
        _turretMaterial.SetFloat(_isBerserkEnabledProperty, 1.0f);
        _flashEffectMaterial.SetFloat(_startTimeFlashProperty, Time.time);

        GameAudioManager.GetInstance().PlayEnterBerserker();
    }
    
    public void StopBerserkVisuals(bool playAudio = true)
    {
        _turretMaterial.SetFloat(_isBerserkEnabledProperty, 0.0f);

        if (playAudio)
        {
            _flashEffectMaterial.SetFloat(_startTimeFlashProperty, Time.time - 0.5f);
            GameAudioManager.GetInstance().PlayExitBerserker();
        }
    }
}
