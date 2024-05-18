using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Context.Scripts.Model.ObjectPool
{
  public interface IObjectPoolerModel
  {
    GameObject SpawnFromPool(string componentTag, Vector3 position, Quaternion rotation);
    Dictionary<string, Queue<GameObject>> poolDictionary { get; set; }
  }
}