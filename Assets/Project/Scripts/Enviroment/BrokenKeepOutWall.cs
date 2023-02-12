using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenKeepOutWall : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_Renderer;

    [SerializeField] private List<float> initIntervalTimes;
    [SerializeField] private List<float> intervalLoopTimes;

    void Start()
    {
        StartCoroutine(InitialBlink());
    }

    private IEnumerator InitialBlink()
    {
        foreach(float interval in initIntervalTimes)
        {
            yield return new WaitForSeconds(interval);
            Action();
        }

        if(intervalLoopTimes.Count > 0)
        {
            StartCoroutine(BlinkBucle());
        }
    }

    private IEnumerator BlinkBucle()
    {
        foreach (float interval in intervalLoopTimes)
        {
            yield return new WaitForSeconds(interval);
            Action();
        }

        StartCoroutine(BlinkBucle());
    }

    private void Action()
    {
        m_Renderer.enabled = !m_Renderer.enabled;
    }
}
