using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapData;

public class OWMap_Node : MonoBehaviour
{
    [SerializeField] private static Color defaultColor = new Color(106f / 255f, 106f / 255f, 106f / 255f);
    [SerializeField] private static Color hoveredColor = new Color(.9f, .9f, .9f);
    [SerializeField] private static Color selectedColor = new Color(38f / 255f, 142f / 255f, 138f / 255f);
    [SerializeField] private static Color destroyedColor = new Color(140f / 255f, 7f / 255f, 36f / 255f);
    [SerializeField] private static Color unavailableColor = new Color(40f / 255f, 7f / 255f, 36f / 255f);

    [SerializeField] private Transform nodeTransform;
    [SerializeField] private BoxCollider interactionCollider;
    [SerializeField] private MeshRenderer meshRenderer;
    private Material material;

    private int state = 0;

    public bool canBeInteractedWith = false;
    


    private void Awake()
    {
        material = meshRenderer.material;
    }


    public void InitTransform(int nodeI, int numLevelNodes, Vector3 mapRightDir, float nodeGapWidth)
    {
        float centerDisplacement = (1f - numLevelNodes) / 2.0f;
        nodeTransform.localPosition = mapRightDir * (nodeI + centerDisplacement) * nodeGapWidth;
    }



    private void OnMouseEnter()
    {
        if (!canBeInteractedWith) return;

        SetHovered();
    }

    private void OnMouseDown()
    {
        state = ++state % 4;

        if (state == 0) SetDefault();
        else if (state == 1) SetSelected();
        else if (state == 2) SetDestroyed();
        else if (state == 3) SetUnavailable();
    }

    public void EnableInteraction()
    {
        interactionCollider.enabled = true;
    }

    public void DisableInteraction()
    {
        interactionCollider.enabled = false;
    }


    public void SetDefault()
    {
        SetColor(defaultColor);
    }

    public void SetHovered()
    {
        SetColor(hoveredColor);
    }

    public void SetSelected()
    {
        SetColor(selectedColor);
    }

    public void SetDestroyed()
    {
        SetColor(destroyedColor);
    }

    public void SetUnavailable()
    {
        SetColor(unavailableColor);
    }


    private void SetColor(Color color)
    {
        material.color = color;
    }
}
