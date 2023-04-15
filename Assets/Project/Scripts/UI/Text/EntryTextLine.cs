using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryTextLine : MonoBehaviour
{
    private IEnumerator clearFunc;

    public void ClearWithTime(float timeToClear, ConsoleDialogSystem consoleDialogSystem)
    {
        clearFunc = Clear(timeToClear, consoleDialogSystem);
        StartCoroutine(clearFunc);
    }

    private IEnumerator Clear(float timeToClear, ConsoleDialogSystem consoleDialogSystem)
    {
        yield return new WaitForSeconds(timeToClear);
        consoleDialogSystem.RemoveSpecificLine(this);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(clearFunc != null)
            StopCoroutine(clearFunc);
    }
}
