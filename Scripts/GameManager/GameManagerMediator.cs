using System.Collections;
using Runtime.Context.Game.Scripts.Models.Panel;
using Runtime.Context.Scripts.Model.Firebase;
using Runtime.Context.Scripts.Model.GameManager;
using Runtime.Context.Scripts.Model.Layer;
using Runtime.Context.Scripts.Model.ScoreManager;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Context.Scripts.View.GameManager
{
  public enum GameEvent
  {
    None,
    Start,
    Replay,
    Pause,
    MainMenu,
    SettingsMenu,
    LogOut,
    GetReward,
    Leaderboard,
    LogIn,
    Quit
  }

  public class GameManagerMediator : EventMediator
  {
    [Inject]
    public GameManagerView view { get; set; }

    [Inject]
    public IScoreManagerModel scoreManagerModel { get; set; }

    [Inject]
    public IGameManagerModel gameManagerModel { get; set; }

    [Inject]
    public IFirebaseModel firebaseModel { get; set; }

    public string highScoreKey = "HighScore";


    public override void OnRegister()
    {
      Time.timeScale = 1;
      scoreManagerModel.score = 0;
      scoreManagerModel.highScore = PlayerPrefs.GetInt(highScoreKey, 0);
      view.dispatcher.AddListener(GameEvent.Replay, OnReplayGame);
      view.dispatcher.AddListener(GameEvent.Quit, OnQuitGame);
      view.dispatcher.AddListener(GameEvent.LogOut, OnLogOut);
      view.dispatcher.AddListener(GameEvent.LogIn, OnLogIn);
    }

    private void OnLogIn()
    {
      SceneManager.LoadScene("Scenes/LoginRegister");
    }

    private void OnLogOut()
    {
      firebaseModel.OnLogOut();
      SceneManager.LoadScene("Scenes/LoginRegister");
    }

    private void Update()
    {
      if (gameManagerModel.isGameOver)
      {
        scoreManagerModel.IncreaseScore(highScoreKey, scoreManagerModel.score);
        gameManagerModel.isGameStarted = false;
      }
    }


    public void OnReplayGame()
    {
      SceneManager.LoadScene("Scenes/Game");
    }

    public void OnQuitGame()
    {
      Application.Quit();
    }


    public override void OnRemove()
    {
      view.dispatcher.RemoveListener(GameEvent.Replay, OnReplayGame);
      view.dispatcher.RemoveListener(GameEvent.Quit, OnQuitGame);
    }
  }
}