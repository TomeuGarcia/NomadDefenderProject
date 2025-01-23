using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class ResultsScreenObjectPreviewer : MonoBehaviour
{
    [SerializeField] private TextDecoder _text;
    [SerializeField] private Transform _previewSpot;
    [SerializeField] private Image _previewImage;

    [SerializeField] private Vector3 _objectOffset = Vector3.zero;
    [SerializeField] private TweenPunchConfig _objectAppearScalePunch;
    [SerializeField] private bool _prantToPreviewer;

    private GameObject _objectToPreview;

    public void InitToNotShow()
    {
        _previewImage.enabled = false;
        gameObject.SetActive(false);
    }
    
    public void InitToShow(Camera camera, GameObject objectToPreview, string appendedText)
    {
        _text.SetTextStrings(_text.textComponent.text += '\n' + appendedText);
        
        if(objectToPreview.GetComponent<Enemy>() != null)
        {
            FindDespicableObjects(objectToPreview.transform);
        }

        _previewImage.enabled = false;
        gameObject.SetActive(true);
        
        Vector3 worldPosition = _previewSpot.position + _objectOffset; //camera.ScreenToWorldPoint(_previewSpot.position);

        _objectToPreview = objectToPreview;
        if (_objectToPreview != null)
        {
            _objectToPreview.transform.position = worldPosition;
            _objectToPreview.SetActive(false);

            if (_prantToPreviewer)
            {
                _objectToPreview.transform.SetParent(_previewSpot);
                _objectToPreview.transform.localRotation = Quaternion.identity;
                _objectToPreview.transform.localPosition = new Vector3(0, -100, 0);
                _objectToPreview.transform.localScale = Vector3.one * 200.0f;
            }
        }
    }

    private void FindDespicableObjects(Transform currentTransform)
    {
        if (currentTransform.gameObject.GetComponent<Light>() != null || currentTransform.gameObject.GetComponent<TrailRenderer>() != null)
        {
            Destroy(currentTransform.gameObject);
            return;
        }

        if (currentTransform.childCount > 0)
        {            
            foreach (Transform t in currentTransform)
            {
                FindDespicableObjects(t);
            }
        }
    }

    public IEnumerator PlayShowAnimation()
    {
        yield return new WaitForSeconds(0.15f);

        if (_objectToPreview != null)
        {
            _objectToPreview.SetActive(true);
            _objectToPreview.transform.PunchScale(_objectAppearScalePunch);
            yield return new WaitForSeconds(0.15f);
        }

        _text.Activate();
        yield return new WaitUntil(() => _text.FinishedLine);
    }
    
}