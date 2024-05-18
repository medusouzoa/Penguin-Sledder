using strange.extensions.mediation.impl;
using UnityEngine;

namespace Runtime.Context.Scripts.View.Camera
{
  public class CameraMediator : EventMediator
  {
    [Inject]
    public CameraView view { get; set; }

    private Vector3 _offset;
    private Transform _transform;

    public override void OnRegister()
    {
      _transform = transform;
      _offset = _transform.position - view.target.position;
    }

    void LateUpdate()
    {
      Vector3 cameraPosition = _transform.position;
      Vector3 newPosition = new Vector3(cameraPosition.x, cameraPosition.y,
        _offset.z + view.target.position.z);
      _transform.position = newPosition;
    }
  }
}