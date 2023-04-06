using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { PATH, GRASS, OBSTACLE }


    [SerializeField] public TileType tileType;
    [SerializeField] private Vector3 buildingPlaceOffset = Vector3.up * 0.2f;
    [HideInInspector] public bool isOccupied = false;

    public Vector3 buildingPlacePosition => transform.position + buildingPlaceOffset;


    public delegate void TileAction(Tile tile);
    public static event TileAction OnTileSelected;
    public static event TileAction OnTileHovered;
    public delegate void TileAction2();
    public static event TileAction2 OnTileUnhovered;


    [HideInInspector] public bool falling = false;


    private void OnMouseDown()
    {
        if (isOccupied) return;

        if (OnTileSelected != null) OnTileSelected(this);
    }

    private void OnMouseEnter()
    {
        if (isOccupied) return;

        if (OnTileHovered != null) OnTileHovered(this);
    }

    private void OnMouseExit()
    {
        if (isOccupied) return;

        if (OnTileUnhovered != null) OnTileUnhovered();
    }


    public IEnumerator FallDown()
    {
        falling = true;
        gameObject.GetComponent<Collider>().enabled = false;
        transform.DOMoveY(-2.0f, 6.0f, false);
        yield return new WaitForSeconds(1.0f);

        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider col in hits)
        {
            if (col.gameObject.GetComponent<PathTile>() != null)
            {
                if (!col.gameObject.GetComponent<PathTile>().falling) StartCoroutine(col.gameObject.GetComponent<PathTile>().FallDown());
            }
            if (col.gameObject.GetComponent<Tile>() != null)
            {
                if (!col.gameObject.GetComponent<Tile>().falling) StartCoroutine(col.gameObject.GetComponent<Tile>().FallDown());
            }
        }
    }
}
