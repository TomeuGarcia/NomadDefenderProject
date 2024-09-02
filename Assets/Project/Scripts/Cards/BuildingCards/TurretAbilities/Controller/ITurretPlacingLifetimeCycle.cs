public interface ITurretPlacingLifetimeCycle
{
    void OnTurretPlacingStart();
    void OnTurretPlacingFinish();
    void OnTurretPlacingMove();
    void OnTurretPlaced(TurretBuilding turretOwner);
}