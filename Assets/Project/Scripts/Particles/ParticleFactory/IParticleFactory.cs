using UnityEngine;

public interface IParticleFactory
{
    Transform ParticleParent { get; }
    Transform Create(ParticleTypes type, Vector3 position, Quaternion rotation, Transform parent = null);
}