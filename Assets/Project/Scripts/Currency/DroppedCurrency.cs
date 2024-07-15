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
    [SerializeField] private Transform meshTransform;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem shatterParticles;


    public delegate void CurrencyAction(int value);
    public static event CurrencyAction OnCurrencyGathered;



    private void OnEnable()
    {
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

        StartCoroutine(StartDisappearing());
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
        GotPickedUp();

        GameAudioManager.GetInstance().PlayCurrencyDropped();
    }

    public void SetValue(int newValue)
    {
        value = newValue;

        //value = Random.Range(1, 4); // Just for testing
        Vector3 minScale = Vector3.one * 0.1f;
        meshTransform.localScale = minScale + ((value -1) * 0.02f * minScale);
    }

    private void GotPickedUp()
    {
        beeingPickedUp = true;

        text.text = "+" + value.ToString();
        text.gameObject.SetActive(true);

        meshRenderer.enabled = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        meshTransform.localPosition = Vector3.zero;
        meshTransform.rotation = Quaternion.identity;
    }



    private IEnumerator StartDisappearing()
    {
        yield return new WaitForSeconds(0.5f);

        meshRenderer.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        shatterParticles.Play();

        yield return new WaitUntil(() => !shatterParticles.gameObject.activeInHierarchy);

        meshRenderer.enabled = true;
        shatterParticles.gameObject.SetActive(true);
        rb.constraints = RigidbodyConstraints.None;
        gameObject.SetActive(false);
    }

}
