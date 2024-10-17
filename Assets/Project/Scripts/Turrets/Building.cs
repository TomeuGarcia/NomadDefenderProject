using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class Building : MonoBehaviour
{
    [Header("Tile Type")]
    [SerializeField] public Tile.TileType validTileType;

    [Header("Building Utils")]
    [SerializeField] protected BuildingsUtils buildingsUtils;

    public BuildingCard.CardBuildingType CardBuildingType { get; protected set; }

    protected bool isFunctional = false;

    public Tile PlacedTile { get; private set; }

    public delegate void BuildingAction();
    public event BuildingAction OnDestroyed;

    public delegate void BuildingAction2(Building invokerBuilding);
    public event BuildingAction2 OnPlaced;
    protected void InvokeOnPlaced() { if (OnPlaced != null) OnPlaced(this); }

    [FormerlySerializedAs("OnTurretUnplaced")] public Action OnBuildingUnplaced;
    
    public BuildingCard BuildingCard { get; protected set; }

    public abstract Vector3 PlacingParticlesPosition { get; }
    
    private void OnDestroy()
    {
        if (OnDestroyed != null) OnDestroyed();
    }


    protected virtual void DisableFunctionality()
    {
        isFunctional = false;
    }

    protected virtual void EnableFunctionality()
    {
        isFunctional = true;
    }

    protected abstract void AwakeInit();

    public void GotPlaced(Tile placedTile)
    {
        PlacedTile = placedTile;
        ServiceLocator.GetInstance().ParticleFactory.Create(ParticleTypes.BuildingPlaced,
            PlacingParticlesPosition, Quaternion.identity);
        DoGotPlaced();
    }

    protected abstract void DoGotPlaced();

    public void GotUnplaced()
    {
        DisableFunctionality();
        gameObject.SetActive(false);
        OnBuildingUnplaced?.Invoke();

        ServiceLocator.GetInstance().ParticleFactory.Create(ParticleTypes.BuildingUnplaced,
            PlacingParticlesPosition, Quaternion.identity);
        DoGotUnplaced();
    }
    protected abstract void DoGotUnplaced();
    
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
