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
    [SerializeField] private GameObject _viewHolder;
    [SerializeField] private Transform _mineHolder;
    [SerializeField] private ParticleSystem _damagedParticles;
    [SerializeField] private ParticleSystem _clearedDestroyParticles;

    [Header("HUD")]
    [SerializeField] private Image _timerFillImage;
    [SerializeField] private GameObject _hudHolder;
    [SerializeField] private Graphic[] _hudHoverGraphics;
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
        
        _timerFillImage.DOKill();
        _timerFillImage.color = _originalFillImageColor;
        
        _viewHolder.SetActive(true);
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

        _timerFillImage.DOComplete();
        _timerFillImage.color = Color.cyan;
        _timerFillImage.DOColor(_originalFillImageColor, 0.3f).SetEase(Ease.InQuad);
    }

    public IEnumerator PlayClearedDestroy()
    {
        _clearedDestroyParticles.Play();
        _hudHolder.SetActive(false);
        yield return new WaitForSeconds(_config.TakeDamageRotationPunch.Duration);
        
        _viewHolder.SetActive(false);
        yield return new WaitUntil(() => !_clearedDestroyParticles.isEmitting);
        yield return new WaitForSeconds(_clearedDestroyParticles.main.startLifetime.constantMax);
    }
    
    public IEnumerator PlayLifetimeEndDestroy()
    {
        _mineHolder.Scale(_config.LifetimeEndScale);
        yield return new WaitForSeconds(_config.TakeDamageRotationPunch.Duration);
    }



    public void ShowHovered()
    {
        foreach (Graphic hudHoverGraphic in _hudHoverGraphics)
        {
            hudHoverGraphic.color = Color.cyan;
        }
    }
    public void HideHovered()
    {
        foreach (Graphic hudHoverGraphic in _hudHoverGraphics)
        {
            hudHoverGraphic.color = Color.white;
        }      
    }
    
}