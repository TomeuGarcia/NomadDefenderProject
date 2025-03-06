using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ChildrenTileRenamer : MonoBehaviour
{
    [SerializeField] private string _namePrefix;
    [SerializeField] private Transform _parent;


    [Button()]
    private void Rename()
    {
        int childCount = _parent.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            _parent.GetChild(i).gameObject.name = _namePrefix + '_' + (i + 1).ToString();
        }
    }
 
    
}
