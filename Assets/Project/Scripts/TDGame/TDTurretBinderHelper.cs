using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDTurretBinderHelper : MonoBehaviour
{
    [SerializeField] private List<TurretBinder> _freeBinders;
    private List<TurretBinder> _occupiedBinders;

    [SerializeField] private BinderMaterial[] _binderTypeMaterial;
    private Dictionary<BinderType, BinderMaterial> _typeToMaterial;


    public enum BinderType { WITHIN_RANGE, OUTSIDE_RANGE, HURT_TARGET, HEAL_TARGET }

    [System.Serializable]
    public class BinderMaterial
    {
        public BinderType binderType;
        public Material material;
    }


    private void Awake()
    {
        _occupiedBinders = new List<TurretBinder>(_freeBinders.Count);
        foreach (TurretBinder binder in _freeBinders)
        {
            binder.Hide();
        }

        _typeToMaterial = new Dictionary<BinderType, BinderMaterial>();
        foreach(BinderMaterial binderMaterial in _binderTypeMaterial)
        {
            _typeToMaterial.Add(binderMaterial.binderType, binderMaterial);
        }

        ServiceLocator.GetInstance().TDTurretBinderHelper = this;
    }


    public TurretBinder TakeBinder(BinderType binderType)
    {
        TurretBinder binder;

        if (_freeBinders.Count == 0)
        {
            binder = _occupiedBinders[0];
        }
        else
        {
            binder = _freeBinders[0];
            _freeBinders.RemoveAt(0);

            _occupiedBinders.Add(binder);
        }

        binder.mesh.material = _typeToMaterial[binderType].material;

        return binder;
    }
    
    public void GiveBackBinder(TurretBinder binder)
    {
        _occupiedBinders.Remove(binder);
        _freeBinders.Add(binder);
    }





}
