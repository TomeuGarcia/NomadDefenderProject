using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorInitSceneLoader : MonoBehaviour
{
#if UNITY_EDITOR
    private static bool firstAwake = true;

    private void Awake()
    {
        if (firstAwake)
        {
            SceneManager.LoadScene(0);
            firstAwake = false;
        }
    }

#endif

}
