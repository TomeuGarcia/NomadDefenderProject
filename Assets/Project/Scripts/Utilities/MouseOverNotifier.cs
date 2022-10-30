using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverNotifier : MonoBehaviour
{
    public delegate void MouseOverAction();
    public event MouseOverAction OnMouseEntered;


    private void OnMouseEnter()
    {
        if (OnMouseEntered != null) OnMouseEntered();
    }
}
