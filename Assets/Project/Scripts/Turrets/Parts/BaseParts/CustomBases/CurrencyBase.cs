using System.Collections;
using UnityEngine;
using static SupportBuilding;

public class CurrencyBase : TurretPartBase_Prefab
{
    [Header("CURRENCY BASE")]
    [SerializeField] private Transform topCube;
    [SerializeField] private MeshRenderer cubeMeshRenderer;
    private Material topCubeMaterial;
    [SerializeField] private float duration;
    private float positionMovement = 0;

    [System.Serializable]
    private struct DropConfigsByLevel
    {
        [SerializeField] private CurrencyDropOverTimeConfig _default;
        [SerializeField] private CurrencyDropOverTimeConfig[] _configsByLevel;

        public CurrencyDropOverTimeConfig GetConfigByLevel(int level)
        {
            if (level < 1)
            {
                return _default;
            }

            return _configsByLevel[level - 1];
        }
    }

    [Header("DROP CONFIG")]
    private CurrencyOverTimeDropper _currencyOverTimeDropper;
    [SerializeField] private DropConfigsByLevel _dropConfigsByLevel;
    [SerializeField] private CurrencyDropOverTimeCanvasView _dropProgressionView;


    private int currentLvl = 0;
    private bool _canGenerateCurrency;


    private void Awake()
    {
        AwakeInit();
        topCubeMaterial = cubeMeshRenderer.material;
        currentLvl = 0;

        _currencyOverTimeDropper = new CurrencyOverTimeDropper(_dropConfigsByLevel.GetConfigByLevel(currentLvl),
            ServiceLocator.GetInstance().CurrencySpawnService, _dropProgressionView);
        _canGenerateCurrency = false;

        _currencyOverTimeDropper.OnCurrencyDropped += OnCurrencyDropped;
    }

    private void OnDestroy()
    {
        _currencyOverTimeDropper.OnCurrencyDropped -= OnCurrencyDropped;
    }

    public override void OnGetPlaced()
    {
        StartCoroutine(CurrencyDropControl());
        _currencyOverTimeDropper.SetSpawnPosition(topCube.position);
    }

    override public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatLevel);

        _currencyOverTimeDropper.SetConfig(_dropConfigsByLevel.GetConfigByLevel(newStatLevel));
    }


    public override void SetDefaultMaterial()
    {
        base.SetDefaultMaterial();
        cubeMeshRenderer.material = topCubeMaterial;
    }

    public override void SetPreviewMaterial()
    {
        base.SetPreviewMaterial();
        cubeMeshRenderer.material = previewMaterials[0][0];

    }

    void Update()
    {
        positionMovement += 0.01f;

        topCube.Rotate(Vector3.up * 0.1f, Space.World);
        topCube.position = new Vector3(topCube.position.x, topCube.position.y + (Mathf.Sin(positionMovement) / 4000.0f), topCube.position.z);
        

        if (_canGenerateCurrency && !AbilityIsDisabled)
        {
            _currencyOverTimeDropper.Update();
        }
    }


    private IEnumerator CurrencyDropControl()
    {
        ITDGameState gameState = ServiceLocator.GetInstance().TDGameState;
        _currencyOverTimeDropper.Start();
        _canGenerateCurrency = true;

        yield return new WaitUntil(() => gameState.GameHasFinished);
        _currencyOverTimeDropper.Finish();
        _canGenerateCurrency = false;
    }


    private void OnCurrencyDropped()
    {
        StartCoroutine(BloomCube());
    }
    private IEnumerator BloomCube()
    {
        float tParam = 0.0f;

        do
        {

            tParam += Time.deltaTime;
            topCubeMaterial.SetFloat("_lerpTime", tParam / duration);
            yield return null;

        } while (tParam < duration);


        yield return null;

        do
        {
            tParam -= Time.deltaTime;
            topCubeMaterial.SetFloat("_lerpTime", tParam / duration);
            yield return null;

        } while (tParam >= 0.0f);

        topCubeMaterial.SetFloat("_lerpTime", 0.0f);
    }
}
