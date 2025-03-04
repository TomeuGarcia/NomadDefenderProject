using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;

public class GUMControl : MonoBehaviour
{
    [SerializeField] private GUMAnimator _GUMAnimator;

    [Header("SCREEN")]
    [SerializeField] private MeshRenderer _screen;
    private Material _screenTransitionMat;
    private Material _screenMat;

    [Header("BUTTON")]
    [SerializeField] private MouseOverNotifier _combineMouseNotifier;
    [SerializeField] private MeshRenderer _combineButtonMesh;
    private Material _combineButtonMat;
    private bool _buttonEnabled;

    [Header("MATERIAL LERP DATA")]
    [SerializeField] private MaterialLerp.FloatData screenTransitionFD;
    [SerializeField] private MaterialLerp.FloatData screenFD;

    public delegate void UpgradeMachineControlAction();
    public event UpgradeMachineControlAction OnReplaceStart;
    public event UpgradeMachineControlAction OnReplaceCardPrinted;

    private void Awake()
    {
        _buttonEnabled = false;
        _combineButtonMat = _combineButtonMesh.material;
        _combineButtonMat.SetFloat("_EnableCoef", 0.0f);
        _combineButtonMat.SetFloat("_AlphaCoef", 0.0f);

        //RenderSettings.reflectionIntensity = 0.0f;

        _combineMouseNotifier.OnMouseEntered += OnHover;
        _combineMouseNotifier.OnMouseExited += OnUnhover;

        StartCoroutine(Enter());
    }

    /*
    private void OnDestroy()
    {
        RenderSettings.reflectionIntensity = 1.0f;
    }
    */


    // BUTTON
    private void OnHover()
    {
        if (_buttonEnabled)
        {
            _combineButtonMat.DOFloat(1.0f, "_HoverCoef", 0.1f);
        }
    }
    private void OnUnhover()
    {
        if (_buttonEnabled)
        {
            _combineButtonMat.DOFloat(0.0f, "_HoverCoef", 0.1f);
        }
    }
    public void ActivateButton()
    {

    }
    public void DeactivateButton()
    {

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
        if (OnReplaceStart != null) OnReplaceStart();
        _GUMAnimator.Upgrading();

        _combineButtonMat.DOFloat(1.0f, "_SelectCoef", 0.1f);
        _combineButtonMat.DOFloat(0.0f, "_AlphaCoef", 0.2f);
    }

    //TODO - CALL THIS FUNCTION FROM ANIMATION TY
    public void ShutDown()
    {
        TODO
        screenTransitionFD.invert = true;
        screenTransitionFD.time = 0.25f;
        StartCoroutine(MaterialLerp.FloatLerp(screenTransitionFD, new Material[1] { _screenTransitionMat }));

        if (OnReplaceCardPrinted != null) OnReplaceCardPrinted();

        _screenMat.DOFloat(0, "_FirstFillCoef", 0.2f);
        _screenMat.DOFloat(0, "_SecondFillCoef", 0.2f);
    }
}
