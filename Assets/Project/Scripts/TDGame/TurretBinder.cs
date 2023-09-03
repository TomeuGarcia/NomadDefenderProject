using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBinder : MonoBehaviour
{
    public MeshRenderer mesh;
    public Transform Transform => mesh.transform;

    public void Show()
    {
        mesh.gameObject.SetActive(true);
    }
    public void Hide()
    {
        mesh.gameObject.SetActive(false);
    }
}
