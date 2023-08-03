using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerTurretBuildingVisuals : TurretBuildingVisuals
{
    Material _material;
    TurretBuilding _owner;


    public void Init(TurretBuilding owner, Material material)
    {
        _owner = owner;
        _material = material;

        SubscribeToEvents();
    }


    private void SubscribeToEvents()
    {
        _owner.OnDestroyed += UnsubscribeToEvents;
        // TODO subscrivbe to events
    }

    private void UnsubscribeToEvents()
    {
        _owner.OnDestroyed -= UnsubscribeToEvents;
        // TODO unsubscrivbe to events
    }

}
