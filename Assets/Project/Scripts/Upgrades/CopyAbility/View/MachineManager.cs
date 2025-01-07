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

    [Header("TWEENS")]
    [SerializeField] private float _startDelay;
    [SerializeField] private float _tubeDelay;

    void Start()
    {
        StartCoroutine(EnterAnimation());
        _replacingAnimation.Init(this);
    }

    private IEnumerator EnterAnimation()
    {
        yield return new WaitForSeconds(_startDelay);

        StartCoroutine(_machineDisplay.EnterAnimation());
        yield return new WaitForSeconds(_tubeDelay);

        StartCoroutine(_rightTube.EnterAnimation());
        StartCoroutine(_cardBase.EnterAnimation());
    }

    [Button]
    private void ReplacingAnimation()
    {
        StartCoroutine(_replacingAnimation.StartAnimation());
    }

    public void LowerTubes()
    {
        StartCoroutine(_rightTube.ReplaceAnimation());
        StartCoroutine(_leftTube.ReplaceAnimation());
    }

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
    }
}
