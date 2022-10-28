using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroppedCurrency : MonoBehaviour
{
    private int value;
    
    [SerializeField] private float indicatorTime;

    private void OnMouseEnter()
    {
        StartCoroutine(GotPickedUp());
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    private IEnumerator GotPickedUp()
    {
        yield return new WaitForSeconds(indicatorTime);
        gameObject.SetActive(false);
    }
}
