using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCurrencyCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        gameObject.transform.parent.gameObject.GetComponent<DroppedCurrency>().Collided(collision);
    }
}
