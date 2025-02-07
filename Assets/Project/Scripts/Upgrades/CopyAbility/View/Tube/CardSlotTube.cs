using System.Collections;
using UnityEngine;

public class CardSlotTube : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private CardSlotTubeArm _arm;
    [SerializeField] private CardSlotHandle[] _handles;
    [SerializeField] private CardSlotGlassTube _glassTube;
    [SerializeField] private CardSlotTubeLid[] _tubeLid;

    [Header("PARAMETERS")]
    [SerializeField] private float _delayToHandle;
    [SerializeField] private float _handlePerDelay;
    [SerializeField] private float _glassTubeDelay;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        StartCoroutine(_arm.EnterAnimation());
        yield return new WaitForSeconds(_delayToHandle);

        for(int i = 0; i < _handles.Length; i++)
        {
            StartCoroutine(_handles[i].EnterAnimation());
            yield return new WaitForSeconds(_handlePerDelay);
        }

        for (int i = 0; i < _tubeLid.Length; i++)
        {
            StartCoroutine(_tubeLid[i].EnterAnimation());
        }
        yield return new WaitForSeconds(_glassTubeDelay);
        StartCoroutine(_glassTube.EnterAnimation());
    }

    public IEnumerator ReplaceAnimation()
    {
        StartCoroutine(_glassTube.CloseTube());
        yield return null;
    }

    public IEnumerator ReopenTube()
    {
        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < _tubeLid.Length; i++)
        {
            _tubeLid[i].Spin();
        }

        StartCoroutine(_glassTube.OpenTube());
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
