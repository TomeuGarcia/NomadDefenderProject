using UnityEngine;

[System.Serializable]
public class TurretCardPartsGroup
{
    [SerializeField] private TurretPartProjectileDataModel _projectile;
    [SerializeField] private TurretPartBody _body;
    
    public TurretPartProjectileDataModel Projectile => _projectile;
    public TurretPartBody Body => _body;

    
    public TurretCardPartsGroup(TurretCardPartsGroup other)
    {
        _projectile = other._projectile;
        _body = other._body;
    }

    public void SetProjectile(TurretPartProjectileDataModel projectile)
    {
        _projectile = projectile;
    }
    public void SetBody(TurretPartBody body)
    {
        _body = body;
    }
}