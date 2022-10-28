using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class DroppedCurrency : MonoBehaviour
{
    private bool beeingPickedUp;

    private int value;

    [Header("COMPONENTS")]
    [SerializeField] private float indicatorTime;

    [Header("COMPONENTS")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private TMP_Text text;

    private void OnMouseEnter()
    {
        if(!beeingPickedUp)
        {
            StartCoroutine(GotPickedUp());
        }
    }

    private void OnEnable()
    {
        beeingPickedUp = false;
        text.gameObject.SetActive(false);

        meshRenderer.enabled = true;
    }

    private void Update()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;

        canvasTransform.rotation = Quaternion.LookRotation(cameraDirection);
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    private IEnumerator GotPickedUp()
    {
        beeingPickedUp = true;

        text.text = "+" + value.ToString();
        text.gameObject.SetActive(true);

        meshRenderer.enabled = false;

        yield return new WaitForSeconds(indicatorTime);
        gameObject.SetActive(false);
    }
}
