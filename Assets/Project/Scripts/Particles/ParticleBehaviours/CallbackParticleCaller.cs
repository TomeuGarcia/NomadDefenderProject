using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Popeye.Modules.VFX.Generic.ParticleBehaviours
{
    public class CallbackParticleCaller : MonoBehaviour
    {
        [SerializeField] private CallbackRecycleParticle _recyclableParticleParent;
        
        void OnParticleSystemStopped()
        {
            _recyclableParticleParent.OnParticleSystemStopped();
        }
    }
}