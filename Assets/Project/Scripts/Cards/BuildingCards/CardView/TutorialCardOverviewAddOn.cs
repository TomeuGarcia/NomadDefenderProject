using System;
using System.Collections;
using UnityEngine;

public class TutorialCardOverviewAddOn : MonoBehaviour
{

    [Header("TUTORIAL OBJECTS")] 
    [SerializeField] private GameObject _object;
    [SerializeField] private TextDecoder _text;

    private void Awake()
    {
        _object.SetActive(false);
    }

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(0.5f);
        
        _object.SetActive(true);
        _text.Activate();
    }

    public void Finish()
    {
        _object.SetActive(false);
    }


}