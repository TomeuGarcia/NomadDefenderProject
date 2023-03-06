using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBattleTutorialMid : MonoBehaviour
{

    [SerializeField] private CardDrawer cardDrawer;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Skip Redraw");
        cardDrawer.OnFinishRedrawsButtonPressed();
    }
}
