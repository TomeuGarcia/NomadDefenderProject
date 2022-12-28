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

    [SerializeField] private static Color slightlyDamagedColor =    new Color(229f / 255f, 200f / 255f, 46f / 255f);
    [SerializeField] private static Color severelyDamagedColor =    new Color(190f / 255f, 80f / 255f, 0f / 255f);
    [SerializeField] private static Color destroyedColor =          new Color(140f / 255f, 7f / 255f, 36f / 255f);


    // INTERACTAVILITY
    // is interactable      ->  player can interact and travel there
    // is NOT interactable  ->  player can't interact nor travel there
    [HideInInspector] public bool isInteractable = false;


    // NODE INTERACT STATE
    // None, Hovered, Selected
    public enum NodeInteractState { NONE, HOVERED, SELECTED }
    [HideInInspector] public NodeInteractState interactState = NodeInteractState.NONE;


    // NODE GAME STATE
    // Undamaged, SlightlyDamaged, SeverelyDamaged, Destroyed
    public enum NodeGameState { UNDAMAGED, SLIGHTLY_DAMAGED, SEVERELY_DAMAGED, DESTROYED }
    [HideInInspector] public NodeGameState gameState = NodeGameState.UNDAMAGED;


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

        SetColor(noInteractionColor);
    }

    public void SetHovered()
    {
        interactState = NodeInteractState.HOVERED;

        SetColor(hoveredColor);
    }

    public void SetSelected()
    {
        interactState = NodeInteractState.SELECTED;

        SetColor(selectedColor);

        DisableInteraction();

        OnNodeSceneExit(); // TODO Move this to corresponding place
    }

    public void SetDestroyed()
    {
        SetColor(destroyedColor);
    }

    private void SetColor(Color color)
    {
        material.color = color;

        if (cameFromConnection != null)
        {
            cameFromConnection.SetColor(color);
        }
    }


    public void SetGameState(NodeGameState nodeGameState)
    {
        switch (nodeGameState)
        {
            case NodeGameState.UNDAMAGED:
                {
                }
                break;
            case NodeGameState.SLIGHTLY_DAMAGED:
                {
                    SetColor(slightlyDamagedColor);
                }
                break;
            case NodeGameState.SEVERELY_DAMAGED:
                {
                    SetColor(severelyDamagedColor);
                }
                break;
            case NodeGameState.DESTROYED:
                {
                    SetColor(destroyedColor);
                }
                break;
            default:
                break;
        }
    }


    private void OnNodeSceneExit()
    {
        if (mapReferencesData.isLastLevelNode)
        {
            Debug.Log("END OF MAP REACHED");
            return; // TODO do something here (END GAME)
        }

        for (int i = 0; i < mapReferencesData.nextLevelNodes.Length; ++i)
        {
            OWMap_Node nextLevelNode = mapReferencesData.nextLevelNodes[i];
            nextLevelNode.SetCameFromConnection(nextLevelConnections[i]);
            nextLevelNode.EnableInteraction();

            nextLevelNode.SetGameState(NodeGameState.UNDAMAGED); // TODO State shouldn't be set here
        }

        foreach (OWMap_Node node in mapReferencesData.levelNeighbourNodes)
        {
            node.DisableInteraction();
        }

        //mapReferencesData.connectionFromLastNode.SetColor(selectedColor);

    }

}
