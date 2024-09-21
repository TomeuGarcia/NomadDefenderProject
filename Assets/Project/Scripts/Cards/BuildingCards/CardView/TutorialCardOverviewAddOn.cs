using System;
using System.Collections;
using UnityEngine;

public class TutorialCardOverviewAddOn : MonoBehaviour
{

    [Header("TUTORIAL OBJECTS")] 
    [SerializeField] private GameObject _object;
    [SerializeField] private TextDecoder _text;

    private bool Finished => !_object.activeInHierarchy;

    private void Awake()
    {
        _object.SetActive(false);
    }

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(0.5f);
        _object.SetActive(true);
        _text.Activate();
        yield return StartCoroutine(CompleteText());
        yield return null;
    }

    private IEnumerator CompleteText()
    {
        yield return new WaitUntil(() => Finished || _text.IsDoneDecoding() || Input.GetKeyDown(KeyCode.Mouse0));

        if (!Finished && !_text.IsDoneDecoding())
        {
            _text.SetStringInstantly();
        }
    }

    public void Finish()
    {
        _object.SetActive(false);
    }


}