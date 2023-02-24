using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_Node : MonoBehaviour
{
    //[SerializeField] public static Color darkGreyColor =   new Color(106f / 255f, 106f / 255f, 106f / 255f);
    //[SerializeField] public static Color lightGreyColor =  new Color(.9f, .9f, .9f);
    //[SerializeField] public static Color blueColor =       new Color(38f / 255f, 142f / 255f, 138f / 255f);

    //[SerializeField] public static Color yellowColor =     new Color(190f / 255f, 190f / 255f, 50f / 255f);
    //[SerializeField] public static Color orangeColor =     new Color(190f / 255f, 80f / 255f, 0f / 255f);
    //[SerializeField] public static Color redColor =        new Color(140f / 255f, 7f / 255f, 36f / 255f);

    //[SerializeField] public static float multiplierColorHDR = 4f;

    private Color colorInUse = OWMapDecoratorUtils.s_darkGreyColor;    
    public Color BorderColor { get; private set; }
    public Texture NodeIconTexture { get; private set; }

    // INTERACTAVILITY
    // is interactable      ->  player can interact and travel there
    // is NOT interactable  ->  player can't interact nor travel there
    [HideInInspector] public bool isInteractable = false;


    // NODE INTERACT STATE
    // None, Hovered, Selected
    public enum NodeInteractState { NONE, HOVERED, SELECTED }
    [HideInInspector] public NodeInteractState interactState = NodeInteractState.NONE;


    // NODE HEALTH STATE
    [HideInInspector] public NodeEnums.HealthState healthState = NodeEnums.HealthState.UNDAMAGED;


    //NODE CLASS
    [HideInInspector] public OWMap_NodeClass nodeClass;
    

    // OW Node Data
    public struct MapReferencesData
    {
        public MapReferencesData(int ownerlevelIndex, int nodeIndexInLevel, 
                                 OWMap_Node[] nextLevelNodes, OWMap_Node[] levelNeighbourNodes)
        {
            this.ownerlevelIndex = ownerlevelIndex;
            this.nodeIndexInLevel = nodeIndexInLevel;
            this.nextLevelNodes = nextLevelNodes;
            this.levelNeighbourNodes = levelNeighbourNodes;

            this.isLastLevelNode = false;
        }

        public MapReferencesData(int ownerlevelIndex, int nodeIndexInLevel, bool isLastLevelNode)
        {
            this.ownerlevelIndex = ownerlevelIndex;
            this.nodeIndexInLevel = nodeIndexInLevel;
            this.nextLevelNodes = null;
            this.levelNeighbourNodes = null;

            this.isLastLevelNode = isLastLevelNode;
        }

        public int ownerlevelIndex;
        public int nodeIndexInLevel;
        public OWMap_Node[] nextLevelNodes;
        public OWMap_Node[] levelNeighbourNodes;
        public bool isLastLevelNode;
    }
    private MapReferencesData mapReferencesData;    
    private OWMap_Connection cameFromConnection;
    private OWMap_Connection[] nextLevelConnections;
    public OWMap_Connection[] GetNextLevelConnections() { return nextLevelConnections; }
    

    [SerializeField] private Transform nodeTransform;
    [SerializeField] private BoxCollider interactionCollider;
    [SerializeField] private MeshRenderer meshRenderer;
    private Material material;
    public const float FADE_DURATION = 0.1f;
    public const float SELECTED_DURATION = 0.075f;
    public const float DEFAULT_BORDER_MOVE_SPEED = 0.05f;
    public const float SELECTED_BORDER_MOVE_SPEED = 0.15f;
    public const float SELECTED_BORDER_MOVE_DURATION = 0.2f;

    //NODE ICON
    private Texture nodeIcon;

    public Vector3 Position => nodeTransform.position;
    public Vector3 Up => nodeTransform.up;
    public Vector3 Forward => nodeTransform.forward;


    private OverworldMapGameManager owMapGameManager;


    [SerializeField] private ParticleSystem particleSystem;


    public void Print()
    {
        Debug.Log("NODE: Lvl-" + mapReferencesData.ownerlevelIndex + ", Idx-" + mapReferencesData.nodeIndexInLevel);
    }


    private void Awake()
    {
        healthState = NodeEnums.HealthState.UNDAMAGED;

        material = meshRenderer.material;

        colorInUse = OWMapDecoratorUtils.s_darkGreyColor;
        material.SetFloat("_TimeOffset", Random.Range(0f, 1f));
        material.SetColor("_BorderColor", OWMapDecoratorUtils.s_darkGreyColor);
        material.SetColor("_IconColor", OWMapDecoratorUtils.s_darkGreyColor);
        material.SetFloat("_IsInteractable", 0f);
        material.SetFloat("_NoiseTwitchingEnabled", 0f);
        material.SetFloat("_IsDamaged", 0f);
        material.SetFloat("_IsDestroyed", 0f);

        material.SetFloat("_FadeDuration", FADE_DURATION);
        material.SetFloat("_TimeStartFade", 0f);
        material.SetFloat("_IsFadingAway", 0f);

        material.SetFloat("_SelectedDuration", SELECTED_DURATION);
        material.SetFloat("_IsSelected", 0f);
        material.SetFloat("_TimeStartSelected", 0f);

        material.SetFloat("_NormalBorderMoveSpeed", DEFAULT_BORDER_MOVE_SPEED);
        material.SetFloat("_FastBorderMoveSpeed", SELECTED_BORDER_MOVE_SPEED);
        material.SetFloat("_FastBorderDuration", SELECTED_BORDER_MOVE_DURATION);
        material.SetFloat("_DoFastBorder", 0f);

        particleSystem.gameObject.SetActive(false);
    }


    public void InitTransform(int nodeI, int numLevelNodes, Vector3 mapRightDir, float nodeGapWidth)
    {
        float centerDisplacement = (1f - numLevelNodes) / 2.0f;
        nodeTransform.localPosition = mapRightDir * (nodeI + centerDisplacement) * nodeGapWidth;
    }

    public void InitMapReferencesData(MapReferencesData mapReferencesData)
    {
        this.mapReferencesData = mapReferencesData;
    }
    public void SetNextLevelConnections(OWMap_Connection[] nextLevelConnections)
    {
        this.nextLevelConnections = nextLevelConnections;
    }
    public void SetCameFromConnection(OWMap_Connection cameFromConnection)
    {
        this.cameFromConnection = cameFromConnection;
    }
    public void SetOwMapGameManagerRef(OverworldMapGameManager owMapGameManager) // To be called from OverworldMapGameManager itself
    {
        this.owMapGameManager = owMapGameManager;
    }

    public MapReferencesData GetMapReferencesData()
    {
        return mapReferencesData;
    }


    private void OnMouseEnter()
    {
        if (!isInteractable) return;

        SetHovered();
    }

    private void OnMouseExit()
    {
        if (!isInteractable) return;

        if (interactState == NodeInteractState.HOVERED)
        {
            SetNotInteracted();
        }
    }

    private void OnMouseDown()
    {
        if (!isInteractable) return;

        SetSelected();
    }

    public void EnableInteraction()
    {
        isInteractable = true;
        interactionCollider.enabled = true;

        material.SetFloat("_IsInteractable", 1f);
        material.SetFloat("_NoiseTwitchingEnabled", 1f);
    }

    public void DisableInteraction()
    {
        isInteractable = false;
        interactionCollider.enabled = false;

        material.SetFloat("_IsInteractable", 0f);
        material.SetFloat("_NoiseTwitchingEnabled", 0f);
    }


    public void SetNotInteracted()
    {
        interactState = NodeInteractState.NONE;

        //SetColor(colorInUse, setCameFromConnectionNotInteracted: true);
        SetIconColor(colorInUse);
        SetCameFromColor(colorInUse, setCameFromConnectionNotInteracted: true);

        SetUnhoveredVisuals();
    }

    public void SetHovered()
    {
        interactState = NodeInteractState.HOVERED;

        SetIconColor(OWMapDecoratorUtils.s_lightGreyColor, true);
        //SetCameFromColor(lightGreyColor);

        SetHoveredVisuals();
    }

    private void SetUnhoveredVisuals()
    {
        material.SetFloat("_IsSelected", 0f);
        material.SetFloat("_TimeStartSelected", Time.time);
    }
    private void SetHoveredVisuals()
    {
        material.SetFloat("_IsSelected", 1f);
        material.SetFloat("_TimeStartSelected", Time.time);
    }
    private void SetSelectedVisuals()
    {
        material.SetFloat("_IsSelected", 1f);
        material.SetFloat("_StartTimeFastBorderMoveSpeed", Time.time);
        material.SetFloat("_DoFastBorder", 1f);
    }

    public void SetSelected()
    {
        interactState = NodeInteractState.SELECTED;

        //SetColor(blueColor);
        SetCameFromColor(OWMapDecoratorUtils.s_blueColor2);

        DisableInteraction();
        if (!mapReferencesData.isLastLevelNode)
        {
            DisableNextLevelNodesInteraction(); // Disable temporarely (to update mouse collisions)
            DisableNeighborLevelNodesInteraction(); // Disable permanently        
        }

        if (owMapGameManager != null)
        {
            owMapGameManager.OnMapNodeSelected(this);
        }

        SetSelectedVisuals();
    }

    private void SetIconColor(Color color, bool mixWithColorInUse = false)
    {
        if (mixWithColorInUse)
        {
            color = Color.Lerp(color, colorInUse, 0.5f);
        }
        //material.SetColor("_IconColor", color * OWMap_Node.multiplierColorHDR);
        material.SetColor("_IconColor", color);

        particleSystem.gameObject.SetActive(true);
        particleSystem.Play();
    }

    public void SetCameFromColor(Color color, bool mixWithDarkGrey = false, bool setCameFromConnectionNotInteracted = false)
    {
        if (mixWithDarkGrey)
        {
            color = Color.Lerp(color, OWMapDecoratorUtils.s_darkGreyColor, 0.5f);
        }

        if (cameFromConnection != null)
        {
            if (setCameFromConnectionNotInteracted)
            {
                cameFromConnection.SetColor(OWMapDecoratorUtils.s_darkGreyColor);
            }
            else
            {
                cameFromConnection.SetColor(color);
            }
        }
    }

    public void SetBorderColor(Color color)
    {
        BorderColor = color;
        //material.SetColor("_BorderColor", color * OWMap_Node.multiplierColorHDR);
        material.SetColor("_BorderColor", color);
    }


    public void SetHealthState(NodeEnums.HealthState nodeHealthState)
    {
        healthState = nodeHealthState;

        switch (healthState)
        {
            case NodeEnums.HealthState.UNDAMAGED:
                {
                    SetIconColor(OWMapDecoratorUtils.s_blueColor);
                    colorInUse = OWMapDecoratorUtils.s_blueColor;
                }
                break;
            case NodeEnums.HealthState.SLIGHTLY_DAMAGED:
                {
                    material.SetFloat("_IsDamaged", 1f);
                    SetIconColor(OWMapDecoratorUtils.s_yellowColor);
                    colorInUse = OWMapDecoratorUtils.s_yellowColor;
                }
                break;
            case NodeEnums.HealthState.GREATLY_DAMAGED:
                {
                    material.SetFloat("_IsDamaged", 1f);
                    SetIconColor(OWMapDecoratorUtils.s_orangeColor);
                    colorInUse = OWMapDecoratorUtils.s_orangeColor;
                }
                break;
            case NodeEnums.HealthState.DESTROYED:
                {
                    material.SetFloat("_IsDamaged", 1f);
                    SetIconColor(OWMapDecoratorUtils.s_redColor);
                    colorInUse = OWMapDecoratorUtils.s_redColor;
                }
                break;
            default:
                break;
        }
    }

    private void DisableNextLevelNodesInteraction()
    {
        for (int i = 0; i < mapReferencesData.nextLevelNodes.Length; ++i)
        {
            OWMap_Node nextLevelNode = mapReferencesData.nextLevelNodes[i];
            nextLevelNode.DisableInteraction();
        }
    }

    public OWMap_Node[] EnableNextLevelNodesInteraction()
    {
        List<OWMap_Node> nextLevelEnabledNodes = new List<OWMap_Node>();

        for (int i = 0; i < mapReferencesData.nextLevelNodes.Length; ++i)
        {
            OWMap_Node nextLevelNode = mapReferencesData.nextLevelNodes[i];
            nextLevelNode.SetCameFromConnection(nextLevelConnections[i]);

            if (nextLevelNode.IsDestroyed())
            {
                nextLevelNode.SetDestroyedVisuals();
            }
            else
            {                
                nextLevelNode.EnableInteraction();

                nextLevelEnabledNodes.Add(nextLevelNode);
            }            
        }

        return nextLevelEnabledNodes.ToArray();
    }

    private void DisableNeighborLevelNodesInteraction()
    {
        foreach (OWMap_Node node in mapReferencesData.levelNeighbourNodes)
        {
            node.DisableInteraction();
        }
    }

    public void SetNodeClass(OWMap_NodeClass _nodeClass, Texture mapIconTexture)
    {
        NodeIconTexture = mapIconTexture;

        nodeClass = _nodeClass;
        nodeClass.SetIconTexture(mapIconTexture);
        nodeIcon = mapIconTexture;

        material.SetTexture("_IconTexture", nodeIcon);
    }

    public NodeEnums.NodeType GetNodeType()
    {
        return nodeClass.nodeType;
    }

    public bool IsDestroyed()
    {
        return healthState == NodeEnums.HealthState.DESTROYED;
    }
    public void SetDestroyedVisuals()
    {
        SetIconColor(OWMapDecoratorUtils.s_redColor);
        SetCameFromColor(OWMapDecoratorUtils.s_redColor);
        material.SetFloat("_IsDamaged", 1f);
        material.SetFloat("_IsDestroyed", 1f);
        material.SetFloat("_NoiseTwitchingEnabled", 1f);
    }


    public void PlayFadeInAnimation()
    {
        material.SetFloat("_TimeStartFade", Time.time);
        material.SetFloat("_IsFadingAway", 0f);
    }

    public void PlayFadeOutAnimation()
    {
        material.SetFloat("_TimeStartFade", Time.time);
        material.SetFloat("_IsFadingAway", 1f);
    }

}
