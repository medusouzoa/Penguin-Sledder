using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Runtime.Context.Game.Scripts.Models.Panel;
using Runtime.Context.Scripts.Enum;
using Runtime.Context.Scripts.Model.Firebase;
using Runtime.Context.Scripts.Model.Layer;
using Runtime.Context.Scripts.View.FirebaseAuth;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Context.Scripts.View.FirebaseRegister
{
  public class FirebaseRegisterMediator : EventMediator
  {
    [Inject]
    public FirebaseRegisterView view { get; set; }

    [Inject]
    public IFirebaseModel firebaseModel { get; set; }

    [Inject]
    public ILayerModel layerModel { get; set; }

    [Inject]
    public IPanelModel panelModel { get; set; }

    private DependencyStatus _dependencyStatus;

    public override void OnRegister()
    {
      firebaseModel.SetConfiguration();
      StartCoroutine(CheckAndFixDependenciesAsync());

      view.dispatcher.AddListener(AuthStateEvent.Register, OnRegisterGame);
      view.dispatcher.AddListener(AuthStateEvent.LogIn, OnLogInGame);
      view.dispatcher.AddListener(AuthStateEvent.MainMenu, OnBackToMainMenu);
      view.dispatcher.AddListener(AuthStateEvent.GoogleRegistry, OnRegisterWithGoogle);
    }

    private void OnLogInGame()
    {
      Destroy(gameObject);
      Transform parent = layerModel.GetLayer(Layers.WelcomeLayer);
      panelModel.LoadPanel(GamePanels.LoginPanel, parent);
    }

    public void OnRegisterGame()
    {
      StartCoroutine(firebaseModel
        .RegisterAsync(view.fullName.text, view.emailLoginField.text,
          view.passwordLoginField.text, view.passwordConfirmationField.text));
    }

    public void OnBackToMainMenu()
    {
      SceneManager.LoadScene("Scenes/Main Menu");
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
      Task<DependencyStatus> dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
      yield return new WaitUntil(() => dependencyTask.IsCompleted);
      _dependencyStatus = dependencyTask.Result;
      if (_dependencyStatus == DependencyStatus.Available)
      {
        yield return new WaitForEndOfFrame();
        StartCoroutine(firebaseModel.CheckForAutoLogin());
      }
      else
      {
        Debug.LogError("Could not resolve all firebase dependencies: " + _dependencyStatus);
      }
    }

    public void OnRegisterWithGoogle()
    {
      firebaseModel.OnSignInWithGoogle();
    }

    public override void OnRemove()
    {
      view.dispatcher.RemoveListener(AuthStateEvent.Register, OnRegisterGame);
      view.dispatcher.RemoveListener(AuthStateEvent.LogIn, OnLogInGame);
      view.dispatcher.RemoveListener(AuthStateEvent.MainMenu, OnBackToMainMenu);
    }
  }
}