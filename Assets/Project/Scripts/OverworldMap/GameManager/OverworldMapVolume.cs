using OWmapShowcase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverworldMapVolume : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    public void ActivateVolume()
    {
        _volume.enabled = true;
        Debug.Log("BBBBBBBBBBBBBBBBBB");
    }

    public void DeactivateVolume()
    {
        _volume.enabled = false;
        Debug.Log("GGGGGGGGGGGGGGG");
    }
}
