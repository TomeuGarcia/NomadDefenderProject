using System.Collections;
using UnityEngine;
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
    
    public void InitToShow(Camera camera, GameObject objectToPreview)
    {
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