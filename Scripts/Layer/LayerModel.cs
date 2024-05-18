using System.Collections.Generic;
using Runtime.Context.Scripts.Enum;
using UnityEngine;

namespace Runtime.Context.Scripts.Model.Layer
{
  public class LayerModel : ILayerModel
  {
    private Dictionary<Layers, Transform> _layers;

    [PostConstruct]
    public void OnPostConstruct()
    {
      _layers = new Dictionary<Layers, Transform>();
    }

    public void AddLayer(Layers key, Transform value)
    {
      Debug.Log("LayerModel>AddLayer> key: " + key + " value: " + value);
      _layers[key] = value;
    }

    public Transform GetLayer(Layers key)
    {
      if (!_layers.ContainsKey(key))
      {
        Debug.LogError(" Player not found in layer map" + key);
        return null;
      }

      Transform transformCont = _layers[key];
      Debug.Log("LayerModel>GetLayer> key: " + key + " value: " + transformCont);

      return transformCont;
    }
  }
}