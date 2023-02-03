using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryTextLine : MonoBehaviour
{
    public IEnumerator ClearWithTime(float timeToClear)
    {
        yield return new WaitForSeconds(timeToClear);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopCoroutine(ClearWithTime(0));
    }
}
