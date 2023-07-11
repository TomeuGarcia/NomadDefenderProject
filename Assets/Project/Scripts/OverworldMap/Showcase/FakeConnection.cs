using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OWmapShowcase
{

    public class FakeConnection : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer[] _meshes;

        public void SetMaterial(Material material)
        {
            foreach (SkinnedMeshRenderer mesh in _meshes)
            {
                mesh.material = material;
            }
        }


    }

}


