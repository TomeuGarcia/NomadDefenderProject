using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager _instance;

    [SerializeField] TextMeshProUGUI textComponent;
    bool mouseHovering = false;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseHovering)
            transform.position = Input.mousePosition;
    }

    public void SetAndShowToolTip(string text)
    {
        mouseHovering = true;
        gameObject.SetActive(true);
        textComponent.text = text;
    }

    public void HideToolTip()
    {
        mouseHovering = false;
        gameObject.SetActive(false);
    }
}
