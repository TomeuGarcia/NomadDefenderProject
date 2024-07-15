using UnityEngine;

public interface IFadingTextCharactersFactory
{
    FadingTextCharacter SpawnFadingTextCharacter(Transform fadingTextParent, char character, Color color);
}
