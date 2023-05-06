using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    [SerializeField] Transform topDoor;
    [SerializeField] Transform botDoor;
    [SerializeField] Transform seal;

    [SerializeField] MeshRenderer lights;
    [SerializeField] Transform pistons;

    public IEnumerator Activate()
    {
        seal.DORotate(new Vector3(seal.rotation.eulerAngles.x, seal.rotation.eulerAngles.y, -180), 0.25f);
        yield return new WaitForSeconds(0.25f);

        seal.DOLocalMoveY(0.25f, 0.4f);
        topDoor.DOLocalMoveY(0.2f, 0.4f);
        botDoor.DOLocalMoveY(0.2f, 0.4f);
        yield return new WaitForSeconds(0.5f);

        topDoor.DOLocalMoveX(0.9f, 0.4f);
        seal.DOLocalMoveX(-0.9f, 0.4f);
        botDoor.DOLocalMoveX(-0.9f, 0.4f);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator Deactivate()
    {
        topDoor.DOLocalMoveX(0f, 0.4f);
        seal.DOLocalMoveX(0f, 0.4f);
        botDoor.DOLocalMoveX(0f, 0.4f);
        yield return new WaitForSeconds(0.5f);

        seal.DOLocalMoveY(0.2f, 0.4f);
        topDoor.DOLocalMoveY(0.15f, 0.4f);
        botDoor.DOLocalMoveY(0.15f, 0.4f);
        yield return new WaitForSeconds(0.75f);

        seal.DORotate(new Vector3(seal.rotation.eulerAngles.x, seal.rotation.eulerAngles.y, 180), 0.25f);
        yield return new WaitForSeconds(0.25f);
    }


}
