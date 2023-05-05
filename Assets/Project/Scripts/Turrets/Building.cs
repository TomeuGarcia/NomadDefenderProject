using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Building : MonoBehaviour
{
    [Header("Tile Type")]
    [SerializeField] public Tile.TileType validTileType;

    [Header("Building Utils")]
    [SerializeField] protected BuildingsUtils buildingsUtils;

    public BuildingCard.CardBuildingType CardBuildingType { get; protected set; }

    protected bool isFunctional = false;


    public delegate void BuildingAction();
    public static event BuildingAction OnBuildingPlaced;
    protected void InvokeOnBuildingPlaced() { if (OnBuildingPlaced != null) OnBuildingPlaced(); }


    protected virtual void DisableFunctionality()
    {
        isFunctional = false;
    }

    protected virtual void EnableFunctionality()
    {
        isFunctional = true;
    }

    protected abstract void AwakeInit();
    public abstract void GotPlaced();
    public abstract void GotEnabledPlacing();
    public abstract void GotDisabledPlacing();
    public abstract void GotMovedWhenPlacing();

    //public virtual void Init(TurretStats turretStats, BuildingCard.BuildingCardParts buildingCardParts)
    //{
    //}



    public abstract void ShowRangePlane();

    public abstract void HideRangePlane();

    public abstract void EnablePlayerInteraction();

    public abstract void DisablePlayerInteraction();

    public virtual void ShowQuickLevelUI() { }

    public virtual void HideQuickLevelUI() { }


    protected Color previewColorInUse;
    public virtual void SetBuildingPartsColor(Color color) { }
    public virtual void SetPreviewCanBePlacedColor() { }
    public virtual void SetPreviewCanNOTBePlacedColor() { }

    public void PlayCanNOTBePlacedColorPunch()
    {
        StartCoroutine(CanNOTBePlacedColorPunch(previewColorInUse, buildingsUtils.PreviewPunchCanNOTBePlacedColor, 0.05f, 2));
    }

    private IEnumerator CanNOTBePlacedColorPunch(Color startColor, Color punchColor, float duration, int times)
    {
        Color currentColor;

        for (int i = 0; i < times; ++i)
        {
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                currentColor = Color.Lerp(startColor, punchColor, t / duration);
                SetBuildingPartsColor(currentColor);

                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                currentColor = Color.Lerp(punchColor, startColor, t / duration);
                SetBuildingPartsColor(currentColor);

                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
        }

        SetBuildingPartsColor(previewColorInUse);
    }


}
