using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OWmapShowcase
{

    public class FakeConnection : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer[] _meshes;
        [SerializeField] private Transform _scaleTransform;
        [SerializeField] private float _baseScale = 1.5f;
        private Vector3 _startScale;

        private void Awake()
        {
            _startScale = _scaleTransform.localScale;
        }

        public void SetMaterial(Material material)
        {
            foreach (SkinnedMeshRenderer mesh in _meshes)
            {
                mesh.material = material;
            }
        }

        public void SetLengthScale(float length)
        {
            length -= 0.7f;
            float lengthScale = length / _baseScale;

            Vector3 newScale = _startScale;
            newScale.z = lengthScale;

            _scaleTransform.localScale = newScale;
        }

    }

}


