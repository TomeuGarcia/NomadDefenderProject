using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    [SerializeField] Transform topDoor;
    [SerializeField] Transform botDoor;
    [SerializeField] Transform seal;

    [SerializeField] List<MeshRenderer> lights = new List<MeshRenderer>();
    [SerializeField] List<Transform> pistons = new List<Transform>();

    [SerializeField] Material offLightMat;
    [SerializeField] Material onLightMat;

    public IEnumerator Activate()
    {
        StartCoroutine(Pistons(-0.55f));
        StartCoroutine(Lights(true));
        seal.DORotate(new Vector3(seal.rotation.eulerAngles.x, seal.rotation.eulerAngles.y, -180), 0.25f);
        GameAudioManager.GetInstance().PlayContainerOpenStart();
        GameAudioManager.GetInstance().PlayContainerSeal();
        yield return new WaitForSeconds(0.25f);

        seal.DOLocalMoveY(0.25f, 0.4f);
        topDoor.DOLocalMoveY(0.2f, 0.4f);
        botDoor.DOLocalMoveY(0.2f, 0.4f);
        yield return new WaitForSeconds(0.5f);

        topDoor.DOLocalMoveX(0.9f, 0.4f);
        seal.DOLocalMoveX(-0.9f, 0.4f);
        botDoor.DOLocalMoveX(-0.9f, 0.4f);
        GameAudioManager.GetInstance().PlayContainerOpenEnd();
        yield return new WaitForSeconds(0.25f);
        GameAudioManager.GetInstance().PlayContainerLightOn();
    }

    public IEnumerator Deactivate()
    {
        topDoor.DOLocalMoveX(0f, 0.4f);
        seal.DOLocalMoveX(0f, 0.4f);
        botDoor.DOLocalMoveX(0f, 0.4f);
        GameAudioManager.GetInstance().PlayContainerOpenEnd();
        yield return new WaitForSeconds(0.5f);

        seal.DOLocalMoveY(0.2f, 0.4f);
        topDoor.DOLocalMoveY(0.15f, 0.4f);
        botDoor.DOLocalMoveY(0.15f, 0.4f);
        GameAudioManager.GetInstance().PlayContainerOpenStart();
        yield return new WaitForSeconds(0.75f);

        StartCoroutine(Lights(false));
        seal.DORotate(new Vector3(seal.rotation.eulerAngles.x, seal.rotation.eulerAngles.y, 180), 0.25f);
        GameAudioManager.GetInstance().PlayContainerSeal();
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(Pistons(-0.5f));
    }

    private IEnumerator Lights(bool open)
    {
        Material matToChange;
        if (open) { matToChange = onLightMat; }
        else { matToChange = offLightMat; }

        foreach (MeshRenderer mesh in lights)
        {
            mesh.material = matToChange;
            GameAudioManager.GetInstance().PlayContainerLightOn();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Pistons(float goalPos)
    {
        foreach (Transform p in pistons)
        {
            p.DOLocalMoveZ(goalPos, 0.5f);
            GameAudioManager.GetInstance().PlayContainerPistonUp();
            yield return new WaitForSeconds(0.2f);
        }
    }
}
