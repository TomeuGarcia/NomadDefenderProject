using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineMovablePart : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }
    public abstract void Init();
    public abstract IEnumerator EnterAnimation();
    public abstract IEnumerator ExitAnimation();
}
