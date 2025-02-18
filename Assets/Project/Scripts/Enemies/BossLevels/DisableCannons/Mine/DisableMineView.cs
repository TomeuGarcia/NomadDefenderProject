using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DisableMineView : MonoBehaviour
{
    [System.Serializable]
    public class Config
    {
        [SerializeField] private TweenPunchConfig _appearScalePunch;
        [SerializeField] private TweenPunchConfig _takeDamageRotationPunch;
        [SerializeField] private TweenConfig _lifetimeEndScale;
        public TweenPunchConfig AppearScalePunch => _appearScalePunch;
        public TweenPunchConfig TakeDamageRotationPunch => _takeDamageRotationPunch;
        public TweenConfig LifetimeEndScale => _lifetimeEndScale;
    }
    
    
    [Header("MINE")]
    [SerializeField] private Transform _mineHolder;
    [SerializeField] private GameObject _hoverHolder;
    [SerializeField] private ParticleSystem _damagedParticles;
    [SerializeField] private ParticleSystem _clearedDestroyParticles;

    [Header("HUD")]
    [SerializeField] private Image _timerFillImage;
    [SerializeField] private GameObject _hudHolder;
    private Color _originalFillImageColor;


    private Config _config;
    

    public void Configure(Config config)
    {
        _config = config;
        _originalFillImageColor = _timerFillImage.color;
    }


    public void Init()
    {
        _mineHolder.DOKill();
        _mineHolder.localScale = Vector3.one;
        _mineHolder.localRotation = Quaternion.identity;
        
        _hudHolder.SetActive(true);
        
        HideHovered();
    }

    public void UpdateTimer(float ratio01)
    {
        _timerFillImage.fillAmount = ratio01;
    }
    
    public void PlayAppearAnimation()
    {
        _mineHolder.PunchScale(_config.AppearScalePunch);
    }
    public void PlayTakeDamageAnimation()
    {
        _damagedParticles.Play();
        _mineHolder.PunchRotation(_config.TakeDamageRotationPunch);

        _timerFillImage.color = Color.cyan;
        _timerFillImage.DOComplete();
        _timerFillImage.DOColor(_originalFillImageColor, 0.3f).SetEase(Ease.InQuad);
    }

    public IEnumerator PlayClearedDestroy()
    {
        _clearedDestroyParticles.Play();
        _hudHolder.SetActive(false);
        yield return new WaitForSeconds(_config.TakeDamageRotationPunch.Duration);
    }
    
    public IEnumerator PlayLifetimeEndDestroy()
    {
        _mineHolder.Scale(_config.LifetimeEndScale);
        yield return new WaitForSeconds(_config.TakeDamageRotationPunch.Duration);
    }



    public void ShowHovered()
    {
        _hoverHolder.SetActive(true);
    }
    public void HideHovered()
    {
        _hoverHolder.SetActive(false);        
    }
    
}