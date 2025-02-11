using UnityEngine;

public class DemoWall : MonoBehaviour
{
    public void Init(OverworldMapCreator owMapCreator, int nodeIndex)
    {
        transform.SetParent(owMapCreator.Holder);
        transform.localPosition = OverworldMapCreator.DisplacementBetweenLevels * (nodeIndex + 0.5f);
    }
}