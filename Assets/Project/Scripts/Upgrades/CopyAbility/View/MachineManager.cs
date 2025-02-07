using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MachineManager : MonoBehaviour
{
    [SerializeField] private CardSlotTube _rightTube;
    [SerializeField] private CardSlotTube _leftTube;
    [SerializeField] private MachineDisplay _machineDisplay;
    [SerializeField] private CardBase _cardBase;
    [SerializeField] private ReplacingAnimation _replacingAnimation;

    [SerializeField] private AudioSource _enterAudioSource;
    [SerializeField] private AudioSource _exitAudioSource;

    [Header("TWEENS")]
    [SerializeField] private float _startDelay;
    [SerializeField] private float _tubeDelay;

    void Start()
    {
        StartCoroutine(EnterAnimation());
        _replacingAnimation.Init(this);
        _enterAudioSource.Play();
    }

    private IEnumerator EnterAnimation()
    {
        yield return new WaitForSeconds(_startDelay);

        StartCoroutine(_machineDisplay.EnterAnimation());
        yield return new WaitForSeconds(_tubeDelay);

        StartCoroutine(_rightTube.EnterAnimation());
        StartCoroutine(_cardBase.EnterAnimation());
    }

    public void LowerTubes()
    {
        StartCoroutine(_rightTube.ReplaceAnimation());
        StartCoroutine(_leftTube.ReplaceAnimation());
    }

    public void ReopenTubes()
    {
        GameAudioManager.GetInstance().PlayContainerOpenEnd();
        StartCoroutine(_rightTube.ReopenTube());
        StartCoroutine(_leftTube.ReopenTube());
    }

    /*
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("PassiveReplicateScene", LoadSceneMode.Single);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            ReplacingAnimation();
        }
    }*/



    [Button]
    public void Warning()
    {
        _machineDisplay.Warning();
    }

    [Button]
    public void Ready()
    {
        _machineDisplay.Ready();
    }

    [Button]
    public void Replace()
    {
        _machineDisplay.Replace();
        StartCoroutine(_replacingAnimation.StartAnimation());
        _exitAudioSource.Play();
    }
}
