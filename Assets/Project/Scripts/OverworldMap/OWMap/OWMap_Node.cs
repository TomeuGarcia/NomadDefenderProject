using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static MapData;

public class OWMap_Node : MonoBehaviour
{
    [SerializeField] private static Color noInteractionColor =      new Color(106f / 255f, 106f / 255f, 106f / 255f);
    [SerializeField] private static Color hoveredColor =            new Color(.9f, .9f, .9f);
    [SerializeField] private static Color selectedColor =           new Color(38f / 255f, 142f / 255f, 138f / 255f);

    [SerializeField] private static Color slightlyDamagedColor =    new Color(170f / 255f, 299f / 255f, 81f / 255f);
    [SerializeField] private static Color greatlyDamagedColor =    new Color(190f / 255f, 80f / 255f, 0f / 255f);
    [SerializeField] private static Color destroyedColor =          new Color(140f / 255f, 7f / 255f, 36f / 255f);

    private Color colorInUse = noInteractionColor;

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
    
    //NODE ICON
    [HideInInspector] public Texture nodeIcon;


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
    

    [SerializeField] private Transform nodeTransform;
    [SerializeField] private BoxCollider interactionCollider;
    [SerializeField] private MeshRenderer meshRenderer;
    private Material material;


    public Vector3 Position => nodeTransform.position;
    public Vector3 Up => nodeTransform.up;
    public Vector3 Forward => nodeTransform.forward;


    private OverworldMapGameManager owMapGameManager;

    public void Print()
    {
        Debug.Log("NODE: Lvl-" + mapReferencesData.ownerlevelIndex + ", Idx-" + mapReferencesData.nodeIndexInLevel);
    }


    private void Awake()
    {
        material = meshRenderer.material;
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
    }

    public void DisableInteraction()
    {
        isInteractable = false;
        interactionCollider.enabled = false;
    }


    public void SetNotInteracted()
    {
        interactState = NodeInteractState.NONE;

        SetColor(colorInUse, setCameFromConnectionNotInteracted: true);
    }

    public void SetHovered()
    {
        interactState = NodeInteractState.HOVERED;

        SetColor(hoveredColor, true);
    }

    public void SetSelected()
    {
        interactState = NodeInteractState.SELECTED;

        SetColor(selectedColor);

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
    }

    public void SetDestroyed()
    {
        SetColor(destroyedColor);
    }

    private void SetColor(Color color, bool mixWithColorInUse = false, bool setCameFromConnectionNotInteracted = false)
    {
        if (mixWithColorInUse)
        {
            material.color = Color.Lerp(color, colorInUse, 0.5f);
        }
        else
        {
            material.color = color;
        }        

        if (cameFromConnection != null)
        {
            if (setCameFromConnectionNotInteracted)
            {
                cameFromConnection.SetColor(noInteractionColor);
            }
            else
            {
                cameFromConnection.SetColor(color);
            }            
        }
    }


    public void SetHealthState(NodeEnums.HealthState nodeHealthState)
    {
        switch (nodeHealthState)
        {
            case NodeEnums.HealthState.UNDAMAGED:
                {
                }
                break;
            case NodeEnums.HealthState.SLIGHTLY_DAMAGED:
                {
                    SetColor(slightlyDamagedColor);
                    colorInUse = slightlyDamagedColor;
                }
                break;
            case NodeEnums.HealthState.GREATLY_DAMAGED:
                {
                    SetColor(greatlyDamagedColor, setCameFromConnectionNotInteracted: true);
                    colorInUse = greatlyDamagedColor;
                }
                break;
            case NodeEnums.HealthState.DESTROYED:
                {
                    SetColor(destroyedColor);
                    colorInUse = destroyedColor;
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

    public void EnableNextLevelNodesInteraction()
    {
        for (int i = 0; i < mapReferencesData.nextLevelNodes.Length; ++i)
        {
            OWMap_Node nextLevelNode = mapReferencesData.nextLevelNodes[i];
            nextLevelNode.SetCameFromConnection(nextLevelConnections[i]);
            nextLevelNode.EnableInteraction();
        }
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
        nodeClass = _nodeClass;
        nodeClass.SetIconTexture(mapIconTexture);
        nodeIcon = mapIconTexture;
    }

    public NodeEnums.NodeType GetNodeType()
    {
        return nodeClass.nodeType;
    }

}
