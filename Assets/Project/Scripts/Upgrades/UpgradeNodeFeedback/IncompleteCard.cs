using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncompleteCard : MonoBehaviour
{
    [SerializeField] private List<TextDecoder> decoders = new List<TextDecoder>();

    private void Start()
    {
        foreach(TextDecoder d in decoders)
        {
            d.Activate();
        }
    }
}
