using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMainCameraAttacher : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        canvas.worldCamera = Camera.main;
    }
}
