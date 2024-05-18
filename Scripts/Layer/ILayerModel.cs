using Runtime.Context.Scripts.Enum;
using UnityEngine;

namespace Runtime.Context.Scripts.Model.Layer
{
  public interface ILayerModel
  {
    void AddLayer(Layers key, Transform value);
    Transform GetLayer(Layers key);
  }
}