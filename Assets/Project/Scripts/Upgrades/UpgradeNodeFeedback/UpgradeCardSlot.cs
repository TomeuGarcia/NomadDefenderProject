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

    private void Awake()
    {
        activePos = transform.position;

        if (!startActivated)
        {
            transform.position -= Vector3.up * deactivatedHeight;
            panel.transform.localPosition = new Vector3(panel.localPosition.x, panel.localPosition.y, insertHeight);
        }
    }

    public IEnumerator Activate()
    {
        lerp.LerpPosition(activePos, activateTime);
        yield return new WaitForSeconds(activateTime + 0.1f);
        Extract();
    }

    public void Insert()
    {
        panelLerp.LerpLocalPosition(new Vector3(panel.localPosition.x, panel.localPosition.y, insertHeight), insertTime);
    }

    public void Extract()
    {
        panelLerp.LerpLocalPosition(new Vector3(panel.localPosition.x, panel.localPosition.y, extractHeight), extractTime);
    }

    public void PulsePanel(int pulse)
    {
        panel.gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_IsOn", pulse);
    }
}
