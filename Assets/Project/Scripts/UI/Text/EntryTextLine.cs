using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryTextLine : MonoBehaviour
{
    private IEnumerator clearFunc;

    public void ClearWithTime(float timeToClear)
    {
        clearFunc = Clear(timeToClear);
        StartCoroutine(clearFunc);
    }

    private IEnumerator Clear(float timeToClear)
    {
        yield return new WaitForSeconds(timeToClear);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(clearFunc != null)
            StopCoroutine(clearFunc);
    }
}
