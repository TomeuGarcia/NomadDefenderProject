using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    public delegate void InitSceneAction();
    public static event InitSceneAction OnStart;

    private IEnumerator Start()
    {
        yield return null;
        if (OnStart != null) OnStart();
    }
}
