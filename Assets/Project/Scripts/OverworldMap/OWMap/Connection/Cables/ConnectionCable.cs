using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ConnectionCable : MonoBehaviour
{
    [SerializeField] protected int cableAmount;
    [SerializeField] protected List<SkinnedMeshRenderer> cableMeshes = new List<SkinnedMeshRenderer>();
    [SerializeField] protected List<GameObject> cableBones = new List<GameObject>();
    protected List<Material> cableMaterials = new List<Material>();

    [SerializeField] protected MaterialLerp.FloatData lerpData;

    private void Awake()
    {
        foreach(SkinnedMeshRenderer mesh in cableMeshes)
        {
            foreach (Material material in mesh.materials)
            {
                cableMaterials.Add(material);
            }
        }
    }

    public virtual void FillCable(bool destroyed)
    {
        if (destroyed) { foreach (Material mat in cableMaterials) { mat.SetFloat("_Broken", 1.0f); mat.SetFloat("_ConnectionCoef", 0.0f); } }
    }

    public virtual void HoverCable() { }

    public void StartNodesConnectionsScaleZToZero()
    {

        foreach (GameObject cableBone in cableBones)
        {
            cableBone.transform.localScale = new Vector3(100.0f, 100.0f, 0.0f);
        }
    }

    public IEnumerator ShowNodesConnections(float timeToLerp)
    {
        foreach (GameObject cableBone in cableBones)
        {
            GameAudioManager.GetInstance().PlayConnectionsNodeSpawnSound();
            cableBone.transform.DOScale(Vector3.one * 100, timeToLerp);
        }
        yield return new WaitForSeconds(timeToLerp);
    }
}
