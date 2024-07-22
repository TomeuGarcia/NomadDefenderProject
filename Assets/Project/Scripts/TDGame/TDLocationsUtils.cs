using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TDLocationsUtils 
{
    public int GetHighestLocationHealth();
    public bool GetHealthiestLocation(Vector3 quearierPosition, out PathLocation healthiestLocation);
    public bool GetMostDamagedLocation(Vector3 quearierPosition, out PathLocation mostDamagedLocation);

}
