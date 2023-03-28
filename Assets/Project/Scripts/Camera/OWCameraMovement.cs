using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class OWCameraMovement : MonoBehaviour
{
    //TODO ACCES LAST NODE POSITION PLS
    //TODO ACCES LAST NODE POSITION PLS
    //TODO ACCES LAST NODE POSITION PLS

    private Vector2 dragRange;

    private Vector3 originalPos;
    private Vector3 nextLevelPos;
    private Vector3 distanceToNextLevel;

    private float lastDragPos;

    private float speed = 0.008f;

    private bool canDrag = false;

    void Update()
    {
        if(canDrag)
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastDragPos = Input.mousePosition.y;
            }
            else if (Input.GetMouseButton(1))
            {
                float difference = Input.mousePosition.y - lastDragPos;

                if (difference == 0) { return; }

                float newPos = Mathf.Clamp(transform.position.z + -difference * speed, dragRange.x, dragRange.y);

                transform.position = new Vector3(transform.position.x, transform.position.y, newPos);

                lastDragPos = Input.mousePosition.y;
            }
        }
    }

    public void Init(Vector3 newDistanceToNextLevel)
    {
        dragRange = new Vector2(transform.position.z, 24.0f); //TODO ACCES LAST NODE POSITION PLS
        distanceToNextLevel = newDistanceToNextLevel;
        canDrag = true;

        SetPositions();
    }

    private void SetPositions()
    {
        originalPos = transform.position;
        nextLevelPos = originalPos + distanceToNextLevel;
    }

    public void MoveToNextLevel()
    {
        Debug.Log("EEAEAEAEAEAE");
        StartCoroutine(LateMoveToNextLevel());
    }

    private IEnumerator LateMoveToNextLevel()
    {
        canDrag = false;
        transform.DOMove(originalPos, 1.5f);
        yield return new WaitForSeconds(1.5f);

        transform.DOMove(nextLevelPos, 2.0f);
        yield return new WaitForSeconds(2.0f);

        SetPositions();
        //canDrag = true;
    }

    public void ResetPosition()
    {
        transform.DOMove(originalPos, 2.0f);
    }
}
