using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseOverNotifier : MonoBehaviour
{
    public delegate void MouseOverAction();
    public event MouseOverAction OnMouseEntered;
    public event MouseOverAction OnMouseExited;
    public event MouseOverAction OnMousePressed;


    private void OnMouseEnter()
    {
        if (OnMouseEntered != null) OnMouseEntered();
    }

    private void OnMouseExit()
    {
        if (OnMouseExited != null) OnMouseExited();
    }


    private void OnMouseDown()
    {
        if (OnMousePressed != null) OnMousePressed();
    }

}
