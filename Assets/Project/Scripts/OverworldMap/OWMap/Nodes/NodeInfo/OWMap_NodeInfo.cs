using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OWMap_NodeInfo : MonoBehaviour
{
    private bool isInteractable = true;

    [Header("DISPLAY")]
    [SerializeField] private GameObject infoCanvasGO;

    private void Awake()
    {
        infoCanvasGO.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (!isInteractable) return;

        ShowNodeInfo();
    }

    private void OnMouseExit()
    {
        if (!isInteractable) return;

        HideNodeInfo();
    }


    private void ShowNodeInfo()
    {
        infoCanvasGO.SetActive(true);
    }


    private void HideNodeInfo()
    {
        infoCanvasGO.SetActive(false);
    }
}
