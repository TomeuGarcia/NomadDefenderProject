using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class EnemyPhotoPersistent : MonoBehaviour
{
    [Header("DO NOT ASSIGN ANYTHING")]
    [SerializeField] private RenderTexture _photoRenderTexture;
    
    [ShowAssetPreview()]
    [SerializeField] private Texture2D _copyTexture;
    public Texture2D PhotoTexture => _copyTexture;
    
    
    private static EnemyPhotoPersistent _instance;
    public static EnemyPhotoPersistent Instance => _instance;
    
    public void Init(RenderTexture photoRenderTexture)
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        transform.SetParent(null);
        _instance = this;
        DontDestroyOnLoad(gameObject);


        _photoRenderTexture = photoRenderTexture;
        _copyTexture = new Texture2D(_photoRenderTexture.width, _photoRenderTexture.height,
            TextureFormat.RGBA64, false);

        RenderTexture previousActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = _photoRenderTexture;
        _copyTexture.ReadPixels(new Rect(0, 0, _photoRenderTexture.width, _photoRenderTexture.height), 0, 0);
        _copyTexture.Apply();
        RenderTexture.active = previousActiveRenderTexture;

    }
}
