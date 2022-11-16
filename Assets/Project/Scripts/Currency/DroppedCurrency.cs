using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class DroppedCurrency : MonoBehaviour
{
    private bool beeingPickedUp;

    private int value;

    [Header("COMPONENTS")]
    [SerializeField] private float indicatorTime;

    [Header("COMPONENTS")]
    [SerializeField] private MouseOverNotifier mouseOverNotifier;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Rigidbody rb;


    public delegate void CurrencyAction(int value);
    public static event CurrencyAction OnCurrencyGathered;



    private void OnEnable()
    {
        //mouseOverNotifier.OnMouseEntered += StartGotPickedUp;
        StartGotPickedUp();

        beeingPickedUp = false;
        text.gameObject.SetActive(false);

        meshRenderer.enabled = true;


        // Apply random forces
        Vector3 randomDir = ((Vector3.right * (Random.Range(-1, 1) < 0 ? -1f : 1f)) + 
                             (Vector3.forward * (Random.Range(-1, 1) < 0 ? -1f : 1f))).normalized;

        Vector3 torqueDir = randomDir * Random.Range(0.5f, 1f);
        rb.AddTorque(torqueDir * 0.2f, ForceMode.Impulse);

        Vector3 forceDir = (Vector3.up * 3f + randomDir).normalized * 3.0f;
        rb.AddForce(forceDir, ForceMode.Impulse);
    }

    private void OnDisable()
    {
        mouseOverNotifier.OnMouseEntered -= StartGotPickedUp;
    }


    private void Update()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;

        canvasTransform.rotation = Quaternion.LookRotation(cameraDirection);
    }

    private void StartGotPickedUp()
    {
        if (beeingPickedUp) return;

        if (OnCurrencyGathered != null) OnCurrencyGathered(value);
        StartCoroutine(GotPickedUp());
    }

    public void SetValue(int newValue)
    {
        value = newValue;

        //value = Random.Range(1, 4); // Just for testing
        Vector3 minScale = Vector3.one * 0.25f;
        meshTransform.localScale = minScale + ((value -1) * 0.2f * minScale);
    }

    private IEnumerator GotPickedUp()
    {
        beeingPickedUp = true;

        text.text = "+" + value.ToString();
        text.gameObject.SetActive(true);

        meshRenderer.enabled = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        meshTransform.localPosition = Vector3.zero;
        meshTransform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(indicatorTime);
        gameObject.SetActive(false);
    }


}
