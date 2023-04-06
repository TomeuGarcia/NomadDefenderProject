using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PathTile : MonoBehaviour
{
    private bool deactivated = false;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material deactivatedMat;
    [SerializeField] List<int> matToChange = new List<int>();

    [HideInInspector] public bool falling = false;
    [SerializeField] GameObject cube;
    [SerializeField] AnimationCurve activateCurve;
    [SerializeField] AnimationCurve deactivateCurve;

    public IEnumerator Deactivate()
    {

        if(deactivated) { yield break; }
        deactivated = true;

        Material[] materials = meshRenderer.materials;
        for(int i = 0; i < matToChange.Count; i++)
        {
            materials[matToChange[i]] = deactivatedMat;
        }
        meshRenderer.materials = materials;

        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider col in hits)
        {
            if (col.gameObject.GetComponent<PathTile>() != null)
            {
                StartCoroutine(col.gameObject.GetComponent<PathTile>().Deactivate());
            }
        }
    }

    public IEnumerator Animation()
    {
        GameObject newCube = Instantiate(cube, transform.parent);
        newCube.transform.position = transform.position;
        newCube.transform.SetParent(transform.parent);
        yield return new WaitForSeconds(0.25f);

        transform.DOMoveY(-100.0f, 50.0f, false);
        yield return new WaitForSeconds(0.75f);

        newCube.gameObject.GetComponent<Lerp>().LerpScale(new Vector3(1.0f, 100.0f, 1.0f), 0.1f);
        yield return new WaitForSeconds(0.75f);

        newCube.gameObject.GetComponent<Lerp>().LerpScale(new Vector3(0.0f, 100.0f, 0.0f), 1.25f);
    }

    public IEnumerator FallDown()
    {
        falling = true;
        gameObject.GetComponent<Collider>().enabled = false;
        transform.DOMoveY(-100.0f, 50.0f, false);
        yield return new WaitForSeconds(1.0f);

        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider col in hits)
        {
            if (col.gameObject.GetComponent<PathTile>() != null)
            {
                if(!col.gameObject.GetComponent<PathTile>().falling) StartCoroutine(col.gameObject.GetComponent<PathTile>().FallDown());
            }
            if (col.gameObject.GetComponent<Tile>() != null)
            {
                if (!col.gameObject.GetComponent<Tile>().falling) StartCoroutine(col.gameObject.GetComponent<Tile>().FallDown());
            }
        }
    }
}
