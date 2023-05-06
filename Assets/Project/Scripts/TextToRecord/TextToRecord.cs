using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToRecord : MonoBehaviour
{
    [SerializeField] private TextDecoder title;
    [SerializeField] private TextDecoder subtitle;

    private bool tActive = false;
    private bool sActive = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            title.Activate();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            subtitle.Activate();
        }
    }
}
