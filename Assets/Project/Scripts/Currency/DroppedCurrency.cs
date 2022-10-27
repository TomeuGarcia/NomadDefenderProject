using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCurrency : MonoBehaviour
{
    private int value;

    private void OnMouseEnter()
    {
        Debug.Log("GOT CLICKED");
        GotPickedUp();
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    private void GotPickedUp()
    {
        gameObject.SetActive(false);
    }
}
