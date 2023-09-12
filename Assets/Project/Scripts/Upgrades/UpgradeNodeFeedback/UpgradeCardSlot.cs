using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardSlot : MonoBehaviour
{
    public bool startActivated;
    private bool active;

    [Header("SLOT")]
    [Header("References")]
    [SerializeField] private Lerp lerp;
    [Header("Parametars")]
    [SerializeField] private float activateTime;
    [SerializeField] private float deactivatedHeight;
    private Vector3 activePos;

    [Header("PANEL")]
    [Header("References")]
    [SerializeField] private Lerp panelLerp;
    [SerializeField] private Transform panel;

    [Header("Parametars")]
    [SerializeField] private float insertTime;
    [SerializeField] private float insertHeight;

    [SerializeField] private float extractTime;
    [SerializeField] private float extractHeight;

    [Header("TRAIL")]
    [Header("References")]
    [SerializeField] private TrailRenderer trail;

    [Header("Parametars")]
    [SerializeField] private float trailTravelTime;

    private Material panelMaterial;
    private float timeLastStart = 0.0f;


    private void Awake()
    {
        activePos = transform.position;

        if (!startActivated)
        {
            transform.position -= Vector3.up * deactivatedHeight;
            panel.transform.localPosition = new Vector3(panel.localPosition.x, panel.localPosition.y, insertHeight);
        }

        panelMaterial = panel.gameObject.GetComponent<MeshRenderer>().materials[0];

        PulsePanel(0);
    }

    public IEnumerator Activate()
    {
        lerp.LerpPosition(activePos, activateTime);
        yield return new WaitForSeconds(activateTime + 0.1f);
        Extract();
        StartCoroutine(TrailMovement());
    }

    public void Insert()
    {
        panelLerp.LerpLocalPosition(new Vector3(panel.localPosition.x, panel.localPosition.y, insertHeight), insertTime);
        trail.emitting = false;
    }

    public void Extract()
    {
        panelLerp.LerpLocalPosition(new Vector3(panel.localPosition.x, panel.localPosition.y, extractHeight), extractTime);
        StartCoroutine(DelayEmission());
    }

    public void ResetStartTime()
    {
        float timeNewStart = Time.time;
        if (timeNewStart - timeLastStart > 0.2f)
        {
            panelMaterial.SetFloat("_StartTime", timeNewStart);
        }                
        timeLastStart = timeNewStart;
    }
    public void PulsePanel(int pulse)
    {
        panelMaterial.SetFloat("_IsOn", pulse);

        trail.emitting = pulse != 0;
    }

    private IEnumerator DelayEmission()
    {
        yield return new WaitForSeconds(extractTime);
        //trail.emitting = true;
    }

    private IEnumerator TrailMovement()
    {
        while (gameObject.activeInHierarchy)
        {
            trail.gameObject.transform.DOLocalMoveX(5.0f, trailTravelTime * 0.8f);
            yield return new WaitForSeconds(trailTravelTime * 0.8f);

            trail.gameObject.transform.DOLocalMoveZ(-5.0f, trailTravelTime);
            yield return new WaitForSeconds(trailTravelTime);

            trail.gameObject.transform.DOLocalMoveX(-5.0f, trailTravelTime * 0.8f);
            yield return new WaitForSeconds(trailTravelTime * 0.8f);
            yield return new WaitForSeconds(0.5f);

            trail.gameObject.transform.DOLocalMoveZ(5.0f, trailTravelTime);
            yield return new WaitForSeconds(trailTravelTime);
        }
    }
}
