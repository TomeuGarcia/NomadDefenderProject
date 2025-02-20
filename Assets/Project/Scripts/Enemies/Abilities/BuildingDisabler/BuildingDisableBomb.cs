using DG.Tweening;
using Scripts.ObjectPooling;
using System.Collections;
using UnityEngine;

public class BuildingDisableBomb : RecyclableObject
{
    [SerializeField] private Transform _EMPTransform;

    private BuildingDisableBombConfig _config;

    private Vector3 _EMPStartPosition;
    private Quaternion _EMPStartRotation;

    private void Awake()
    {
        _EMPStartPosition = _EMPTransform.position;
        _EMPStartRotation = _EMPTransform.localRotation;
    }

    internal override void RecycledInit()
    {

    }

    internal override void RecycledReleased()
    {

    }

    public void Init(BuildingDisableBombConfig config)
    {
        _config = config;

        _EMPTransform.localPosition = _EMPStartPosition;
        _EMPTransform.localRotation = _EMPStartRotation;
        _EMPTransform.gameObject.SetActive(true);

        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        StartCoroutine(PlayAnimation());
        StartCoroutine(SpawnWave());

        yield return new WaitForSeconds(Mathf.Max(_config.DropAnimation.Duration, _config.DelayBeforeApplying));
        Recycle();
    }

    private IEnumerator PlayAnimation()
    {
        _EMPTransform.DOLocalMove(_config.DropAnimation.EMPEndPosition, _config.DropAnimation.Duration).SetEase(_config.DropAnimation.Ease);
        _EMPTransform.DOLocalRotate(_config.DropAnimation.EMPEndRotation, _config.DropAnimation.Duration).SetEase(_config.DropAnimation.Ease);
        yield return new WaitForSeconds(_config.DropAnimation.Duration);
        _EMPTransform.gameObject.SetActive(false);
    }

    private IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(_config.DelayBeforeApplying);
        BuildingDisableWaveFactory.Instance.Create(_config.WaveConfig, transform.position, Quaternion.identity);
    }
}
