using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReplacingAnimationTube : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _tubeFlash;
    [SerializeField] private Light _lightFlash;
    [SerializeField] private GameObject _ambientLightParent;
    [SerializeField] private ParticleSystem _leftParticleSystem;
    [SerializeField] private ParticleSystem _rightParticleSystem;

    [Header("PARAMETERS")]
    [SerializeField] private float _turnOffLightDelay;
    [SerializeField] private float _turnOnLightDelay;
    [SerializeField] private TweenConfig _inTubeFlashConfig;
    [SerializeField] private TweenConfig _inLightFlashConfig;
    [SerializeField] private TweenConfig _inSceneLightIntensity;

    [SerializeField] private TweenConfig _outTubeFlashConfig;
    [SerializeField] private TweenConfig _outLightFlashConfig;
    [SerializeField] private TweenConfig _outSceneLightIntensity;
    [SerializeField] private List<float> _ambientLightBlinkInterval = new();

    [Header("PARTICLES")]
    [SerializeField] private TweenConfig _leftAirConfig;
    [SerializeField] private TweenConfig _leftAirSpeedConfig;

    public IEnumerator PlayTubeFlash()
    {
        DOTween.To(() => RenderSettings.ambientIntensity, x => RenderSettings.ambientIntensity = x, _inSceneLightIntensity.Value.x, _inSceneLightIntensity.Duration)
            .SetEase(_inSceneLightIntensity.Ease);
        _ambientLightParent.SetActive(false);
        yield return new WaitForSeconds(_turnOffLightDelay);

        _lightFlash.DOIntensity(_inLightFlashConfig.Value.x, _inLightFlashConfig.Duration).SetEase(_inLightFlashConfig.Ease);
        _tubeFlash.DOScale(_inTubeFlashConfig.Value, _inTubeFlashConfig.Duration).SetEase(_inTubeFlashConfig.Ease);
        yield return new WaitForSeconds(_inTubeFlashConfig.Duration);

        _lightFlash.DOIntensity(_outLightFlashConfig.Value.x, _outLightFlashConfig.Duration).SetEase(_outLightFlashConfig.Ease);
        _tubeFlash.DOScale(_outTubeFlashConfig.Value, _outTubeFlashConfig.Duration).SetEase(_outTubeFlashConfig.Ease);
        yield return new WaitForSeconds(_turnOnLightDelay);

        DOTween.To(() => RenderSettings.ambientIntensity, x => RenderSettings.ambientIntensity = x, _outSceneLightIntensity.Value.x, _outSceneLightIntensity.Duration)
            .SetEase(_outSceneLightIntensity.Ease);
        foreach (float interval in _ambientLightBlinkInterval)
        {
            yield return new WaitForSeconds(interval);
            _ambientLightParent.SetActive(!_ambientLightParent.activeInHierarchy);
        }
        _ambientLightParent.SetActive(true);
    }

    public void ParticleAcceleration()
    {
        ActivateTubeParticles(_leftParticleSystem);
        ActivateTubeParticles(_rightParticleSystem);
    }

    public void ActivateTubeParticles(ParticleSystem particleSystem)
    {
        var trailModule = particleSystem.trails;
        DOTween.To(() => trailModule.lifetime.constant, x => trailModule.lifetime = x, _leftAirConfig.Value.x, _leftAirConfig.Duration)
            .SetEase(_leftAirConfig.Ease)
            .OnComplete(() => StartCoroutine(StopLeftParticles(particleSystem)));

        var mainModule = particleSystem.main;
        DOTween.To(() => mainModule.startSpeed.constant, x => mainModule.startSpeed = x, _leftAirSpeedConfig.Value.x, _leftAirSpeedConfig.Duration)
            .SetEase(_leftAirSpeedConfig.Ease);
    }

    private IEnumerator StopLeftParticles(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(1.0f);
        particleSystem.Stop();
    }
}
