using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;

public class GUMControl : MonoBehaviour
{
    [SerializeField] private GUMAnimator _GUMAnimator;
    [SerializeField] private UpgradeMachineControl _upgradeMachineControl;

    [Header("SCREEN")]
    [SerializeField] private MeshRenderer _screenMesh;
    [SerializeField] private MeshRenderer _screenTransitionMesh;
    private Material _screenTransitionMat;
    private Material _screenMat;

    [Header("BUTTON")]
    [SerializeField] private MouseOverNotifier _combineMouseNotifier;
    [SerializeField] private MeshRenderer _combineButtonMesh;
    private Material _combineButtonMat;
    private bool _buttonEnabled;

    [Header("MATERIAL LERP DATA")]
    [SerializeField] private MaterialLerp.FloatData screenTransitionFD;

    private void Awake()
    {
        _buttonEnabled = false;
        _combineButtonMat = _combineButtonMesh.material;
        _combineButtonMat.SetFloat("_EnableCoef", 0.0f);
        _combineButtonMat.SetFloat("_AlphaCoef", 0.0f);

        _screenMat = _screenMesh.materials[1];
        _screenTransitionMat = _screenTransitionMesh.materials[0];

        RenderSettings.reflectionIntensity = 0.0f;

        _combineMouseNotifier.OnMouseEntered += OnHover;
        _combineMouseNotifier.OnMouseExited += OnUnhover;
    }

    private void Start()
    {
        StartCoroutine(Enter());
    }

    private void OnDestroy()
    {
        RenderSettings.reflectionIntensity = 1.0f;
    }


    // BUTTON
    private void OnHover()
    {
        if (_buttonEnabled)
        {
            _combineButtonMat.DOFloat(1.0f, "_HoverCoef", 0.1f);
            GameAudioManager.GetInstance().PlayCardHovered();
        }
    }
    private void OnUnhover()
    {
        if (_buttonEnabled)
        {
            _combineButtonMat.DOFloat(0.0f, "_HoverCoef", 0.1f);
            GameAudioManager.GetInstance().PlayCardHoverExit();
        }
    }
    public void ActivateButton()
    {
        screenTransitionFD.invert = false;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { _screenTransitionMat }));

        _combineButtonMat.DOFloat(1.0f, "_EnableCoef", 0.2f);
        _buttonEnabled = true;
    }
    public void DeactivateButton()
    {
        screenTransitionFD.invert = true;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { _screenTransitionMat }));

        _combineButtonMat.DOFloat(0.0f, "_EnableCoef", 0.2f);
        _buttonEnabled = false;
    }


    // GUM ANIMATOR
    public IEnumerator Enter()
    {
        _GUMAnimator.Enter();
        yield return new WaitForSeconds(2.0f);
        _combineButtonMat.DOFloat(1.0f, "_AlphaCoef", 0.2f);
    }

    public void LeftReady()
    {
        _GUMAnimator.LeftReady();
    }
    public void LeftUnReady()
    {
        _GUMAnimator.LeftUnReady();
    }

    public void RightReady()
    {
        _GUMAnimator.RightReady();
    }
    public void RightUnReady()
    {
        _GUMAnimator.RightUnReady();
    }

    public void Ready()
    {
        _GUMAnimator.Ready();
    }
    public void UnReady()
    {
        _GUMAnimator.UnReady();
    }

    public void Upgrading()
    {
        GameAudioManager.GetInstance().PlayUpgradeButtonPressed();
        _GUMAnimator.Upgrading();

        _combineButtonMat.DOFloat(1.0f, "_SelectCoef", 0.1f);
        _combineButtonMat.DOFloat(0.0f, "_AlphaCoef", 0.2f);
    }

    public void ShutDown()
    {
        _GUMAnimator.ShutDown();
        screenTransitionFD.invert = true;
        screenTransitionFD.time = 0.25f;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { _screenTransitionMat }));

        _upgradeMachineControl.ShutDown();

        _screenMat.DOFloat(0, "_FirstFillCoef", 0.2f);
        _screenMat.DOFloat(0, "_SecondFillCoef", 0.2f);
    }
}
