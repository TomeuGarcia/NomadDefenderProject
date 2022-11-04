using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardWorldCanvas : MonoBehaviour
{

    [SerializeField] private RectTransform canvasTransform;


    private void Update()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        canvasTransform.rotation = Quaternion.LookRotation(cameraDirection);
    }

}
