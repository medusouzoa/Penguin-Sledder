using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Context.Scripts.Model.ObjectPool
{
  public class ObjectPoolerModel : IObjectPoolerModel
  {
    public Dictionary<string, Queue<GameObject>> poolDictionary { get; set; }

    public GameObject SpawnFromPool(string componentTag, Vector3 position, Quaternion rotation)
    {
      if (!poolDictionary.ContainsKey(componentTag))
      {
        Debug.LogWarning("Pool with tag " + componentTag + " doesn't exists.");
        return null;
      }

      GameObject objectToSpawn = poolDictionary[componentTag].Dequeue();
      objectToSpawn.SetActive(true);
      objectToSpawn.transform.position = position;
      objectToSpawn.transform.rotation = rotation;
      poolDictionary[componentTag].Enqueue(objectToSpawn);
      return objectToSpawn;
    }
  }
}