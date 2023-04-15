using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class OWMap_Connection : MonoBehaviour
{
    [SerializeField] private Transform connectionTransform;

    public ConnectionCable cable { get; private set; }

    public void InitTransform(Vector3 cNodeLocalPos, Vector3 nNodeLocalPos, Vector3 mapForwardDir)
    {
        Vector3 currentToNext = nNodeLocalPos - cNodeLocalPos;
        Vector3 dirCurrentToNext = currentToNext.normalized;

        Vector3 currentToNextMidPoint = cNodeLocalPos + (dirCurrentToNext * (currentToNext.magnitude / 2.0f));

        connectionTransform.localPosition = currentToNextMidPoint;
    }

    public void SetConnection(GameObject connectionCable, bool inverted)
    {
        cable = connectionCable.gameObject.GetComponent<ConnectionCable>();
        cable.transform.SetParent(gameObject.transform, false);
        
        if(inverted)
        {
            cable.transform.localPosition += Vector3.left * (cable.transform.localPosition.x * 2.0f);
            cable.transform.Rotate(Vector3.forward, 180);//cable.transform.rotation = Quaternion.Euler(cable.transform.rotation.x, cable.transform.rotation.y, cable.transform.rotation.z + 180f);
        }
    }

    public void LightConnection(bool destroyed)
    {
        if(destroyed)
        {
            StartCoroutine(DestroyedNode());
        }
        else
        {
            cable.FillCable(false);
        }
    }

    public IEnumerator DestroyedNode()
    {
        yield return new WaitForSeconds(2.0f);
        cable.FillCable(true);
    }

    public void HoverConnection()
    {
        cable.HoverCable();
    }


}
