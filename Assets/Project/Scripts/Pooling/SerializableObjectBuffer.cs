using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableObjectBuffer
{
    [SerializeField] private List<GameObject> _spawnedObjectsBuffer;
    private Queue<GameObject> _spawnedObjectsLeftBuffer;

    public void SetupBeforeUse()
    {
        _spawnedObjectsLeftBuffer = new Queue<GameObject>(_spawnedObjectsBuffer);
        _spawnedObjectsBuffer.Clear();
    }
    public void ClearAfterUse()
    {
        while (_spawnedObjectsLeftBuffer.Count > 0)
        {
            GameObject.DestroyImmediate(_spawnedObjectsLeftBuffer.Dequeue());
        }
    }

    public bool HasObjectsLeft(out GameObject nextObject)
    {
        return _spawnedObjectsLeftBuffer.TryDequeue(out nextObject);
    }

    public void AddToSpawnedObjectsBuffer(GameObject spawnedObject)
    {
        _spawnedObjectsBuffer.Add(spawnedObject);
    }

    public void DestroyObjectsAndClearBuffer()
    {
        foreach (GameObject bufferObject in _spawnedObjectsBuffer)
        {
            GameObject.DestroyImmediate(bufferObject);
        }
        _spawnedObjectsBuffer.Clear();
    }
}