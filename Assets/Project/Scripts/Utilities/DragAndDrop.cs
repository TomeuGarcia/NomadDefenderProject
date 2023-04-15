using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    float yInc;
    private Vector3 prevMousePosition;
    private Vector3 mousePosition;

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePosition();
        prevMousePosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);

        float totalHeightToMove = 10f / (float)Camera.main.pixelHeight;
        yInc += (Input.mousePosition.y - prevMousePosition.y) * totalHeightToMove;
        yInc = Mathf.Clamp01(yInc);
        prevMousePosition = Input.mousePosition;
        
        Debug.Log(yInc);
    }

    private Vector3 GetMousePosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

}
