using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMapTutorialManagerOptional : MonoBehaviour
{

    [SerializeField] private ScriptedSequence scriptedSequence;
    [SerializeField] private OWCameraMovement cameraMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Tutorial()
    {
        cameraMovement.CanDrag(false);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //0 -> Computing Battle Result...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //0 -> Computing Battle Result...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.Clear();
        cameraMovement.CanDrag(true);
    }
}
