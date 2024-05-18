using System.Collections;
using Runtime.Context.Game.Scripts.Models.Panel;
using Runtime.Context.Scripts.Enum;
using Runtime.Context.Scripts.Model.GameManager;
using Runtime.Context.Scripts.Model.Layer;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Runtime.Context.Scripts.View.Player
{
  public class PlayerMediator : EventMediator
  {
    [Inject]
    public PlayerView view { get; set; }

    [Inject]
    public IGameManagerModel gameManagerModel { get; set; }

    [Inject]
    public ILayerModel layerModel { get; set; }

    [Inject]
    public IPanelModel panelModel { get; set; }

    private Animator _animator;
    private bool _isSliding;
    private CharacterController _characterController;
    private int _desiredLane = 1; // 0 left 1 middle 2 right
    public Vector3 direction;
    private float _endTime;
    private float _lastTapTime = 0;

    public static bool tap;
    private static bool _swipeLeft, _swipeRight, _swipeUp, _swipeDown;
    private bool _isDragging = false;
    private Vector2 _startTouch, _swipeDelta;

    public override void OnRegister()
    {
      _characterController = GetComponent<CharacterController>();
      _animator = GetComponent<Animator>();
      _isSliding = false;
      gameManagerModel.player = gameObject;
    }

    private void Update()
    {
      if (gameManagerModel.isGameStarted)
      {
        HandleInput();
        CalculateSwipeDelta();
        CheckSwipeMagnitude();
        HandleSwipeInputs();
      }

      if (!gameManagerModel.isGameStarted)
      {
        _animator.SetBool("isRunning", false);
        return;
      }

      // Update animator state
      _animator.SetBool("isRunning", true);
      _animator.SetBool("isJumping", !_characterController.isGrounded);

      // Check for double tap
      DetectDoubleTap();


      if (gameManagerModel.isGameStarted)
      {
        // Increment forward speed
        view.forwardSpeed += 0.1f * Time.deltaTime;
        direction.z = view.forwardSpeed;

        // Move player towards target position
        transform.position = Vector3.MoveTowards(transform.position,
          CalculateTargetPosition(), view.laneChangeSpeed * Time.deltaTime);
        _characterController.center = _characterController.center;
      }
    }

    private void CalculateSwipeDelta()
    {
      _swipeDelta = Vector2.zero;

      if (_isDragging)
      {
        if (Input.touches.Length > 0)
          _swipeDelta = Input.touches[0].position - _startTouch;
        else if (Input.GetMouseButton(0))
          _swipeDelta = (Vector2)Input.mousePosition - _startTouch;
      }
    }

    private void HandleSwipeInputs()
    {
      if (_characterController.isGrounded)
      {
        _animator.SetBool("isJumping", false);
        if (_swipeUp)
        {
          Jump();
          Debug.Log("Swipe Up");
        }
      }
      else
      {
        direction.y += view.gravity * Time.deltaTime * 1.2f;
      }

      if (_swipeDown)
      {
        StartCoroutine(Slide());
        if (!_characterController.isGrounded)
        {
          direction.y += view.gravity * 0.4f;
        }

        Debug.Log("Swipe Down");
      }

      else if (_swipeLeft)
      {

        _desiredLane--;
        _desiredLane = Mathf.Clamp(_desiredLane, 0, 2);

        if (_isSliding)
        {
          StopCoroutine(Slide());
          _isSliding = false;
          _animator.SetBool("isSliding", false);
        }

        Debug.Log("Swipe Left");
      }

      else if (_swipeRight)
      {
        _desiredLane++;
        _desiredLane = Mathf.Clamp(_desiredLane, 0, 2);
        if (_isSliding)
        {
          StopCoroutine(Slide());
          _isSliding = false;
          _animator.SetBool("isSliding", false);
        }

        Debug.Log("Swipe Right");
      }
    }

    private void CheckSwipeMagnitude()
    {
      if (_swipeDelta.magnitude > 70)
      {
        float x = _swipeDelta.x;
        float y = _swipeDelta.y;

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
          if (x < 0)
            _swipeLeft = true;
          else
            _swipeRight = true;
        }
        else
        {
          if (y < 0)
            _swipeDown = true;
          else
            _swipeUp = true;
        }

        Reset();
      }
    }


    private void HandleInput()
    {
      ResetFlags();

      if (Input.GetMouseButtonDown(0))
      {
        tap = true;
        _isDragging = true;
        _startTouch = Input.mousePosition;
      }
      else if (Input.GetMouseButtonUp(0))
      {
        _isDragging = false;
        Reset();
      }
      else if (Input.touches.Length > 0)
      {
        if (Input.touches[0].phase == TouchPhase.Began)
        {
          tap = true;
          _isDragging = true;
          _startTouch = Input.touches[0].position;
        }
        else if (Input.touches[0].phase == TouchPhase.Ended ||
                 Input.touches[0].phase == TouchPhase.Canceled)
        {
          _isDragging = false;
          Reset();
        }
      }
    }


    private void DetectDoubleTap()
    {
      if (Input.touchCount == 1)
      {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
          if (Time.time - _lastTapTime <= view.doubleTapTimeThreshold)
          {
            _lastTapTime = 0;
            Debug.Log("Double tap detected");
          }
          else
          {
            _lastTapTime = Time.time;
          }
        }
      }
    }

    private Vector3 CalculateTargetPosition()
    {
      Vector3 targetPosition = transform.position;

      float targetLaneXPosition = 0f;

      if (_desiredLane == 0)
      {
        targetLaneXPosition = -view.laneDistance;
      }
      else if (_desiredLane == 2)
      {
        targetLaneXPosition = view.laneDistance;
      }

      targetPosition.x = targetLaneXPosition;

      return targetPosition;
    }

    private IEnumerator Slide()
    {
      _isSliding = true;
      _animator.SetBool("isSliding", true);
      _characterController.center = new Vector3(0, 1.3f, -0.3f);
      _characterController.height = 0.8f;

      float slideDuration = 0.6f;
      float slideTimer = 0f;

      Vector3 initialCenter = new Vector3(0, 1.3f, -0.3f);
      Vector3 targetCenter = new Vector3(0, 0.8f, -0.6f);
      float initialHeight = 0.8f;
      float targetHeight = 1.6f;

      while (slideTimer < slideDuration)
      {
        slideTimer += Time.deltaTime;
        float t = slideTimer / slideDuration;
        _characterController.center = Vector3.Lerp(initialCenter, targetCenter, t);
        _characterController.height = Mathf.Lerp(initialHeight, targetHeight, t);
        yield return null;
      }

      _characterController.center = targetCenter;
      _characterController.height = targetHeight;
      _animator.SetBool("isSliding", false);
      _isSliding = false;
    }

    private void Jump()
    {
      direction.y = view.jumpForce * 1.2f;
      _animator.SetBool("isJumping", true);
    }

    void FixedUpdate()
    {
      if (!gameManagerModel.isGameStarted)
      {
        return;
      }

      _characterController.Move(direction * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("obstacle"))
      {
        gameManagerModel.isGameStarted = false;
        _desiredLane = 1;
        Transform parent = layerModel.GetLayer(Layers.Hud);
        GameObject parentGameObject = parent.gameObject;
        DeleteAllChildren(parentGameObject);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x,
          10f, gameObject.transform.position.z);
        panelModel.LoadPanel(GamePanels.GameOverPanel, parent);
        Debug.Log("The end");
      }
    }

    public void DeleteAllChildren(GameObject parentObject)
    {
      // Check if the parent GameObject is valid
      if (parentObject != null)
      {
        // Loop through all child objects and destroy them
        foreach (Transform child in parentObject.transform)
        {
          Destroy(child.gameObject);
        }
      }
      else
      {
        Debug.LogWarning("Parent GameObject reference not set.");
      }
    }

    private void Reset()
    {
      _startTouch = _swipeDelta = Vector2.zero;
      _isDragging = false;
    }

    private void ResetFlags()
    {
      tap = _swipeLeft = _swipeRight = _swipeUp = _swipeDown = false;
    }
  }
}