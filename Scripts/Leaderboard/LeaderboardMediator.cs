using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using Runtime.Context.Game.Scripts.Models.Panel;
using Runtime.Context.Scripts.Enum;
using Runtime.Context.Scripts.Model.Firebase;
using Runtime.Context.Scripts.Model.Layer;
using Runtime.Context.Scripts.Vo;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Runtime.Context.Scripts.View.Leaderboard
{
  public enum LeaderboardEvent
  {
    Close,
    None
  }

  public class LeaderboardMediator : EventMediator
  {
    public DatabaseReference databaseReference;

    [Inject]
    public LeaderboardView view { get; set; }

    [Inject]
    public IFirebaseModel firebaseModel { get; set; }

    public override void OnRegister()
    {
      view.dispatcher.AddListener(LeaderboardEvent.Close, OnCloseTab);
      databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
      FetchLeaderboard();
      if (!firebaseModel.CheckForUser())
      {
        Instantiate(view.loginSlot, view.loginContainer);
      }
    }

    private void OnCloseTab()
    {
      Destroy(gameObject);
    }


    public void FetchLeaderboard()
    {
      databaseReference.Child("leaderboard").OrderByChild("score")
        .LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to fetch leaderboard data");
          }
          else if (task.IsCompleted)
          {
            DataSnapshot snapshot = task.Result;
            List<LeaderboardDataVo> leaderboardData = new List<LeaderboardDataVo>();

            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
              string userId = childSnapshot.Key;
              string fullName = childSnapshot.Child("playerName").Value.ToString();
              int money = int.Parse(childSnapshot.Child("score").Value.ToString());

              leaderboardData.Add(new LeaderboardDataVo
              {
                playerName = fullName,
                score = money, id = userId
              });
            }

            leaderboardData.Reverse();
            int degree = 1;
            foreach (LeaderboardDataVo playerData in leaderboardData)
            {
              GameObject instantiatedObject = Instantiate(view.rowSlot, view.container);
              LeaderboardRowVo leaderboardRow = instantiatedObject.GetComponent<LeaderboardRowVo>();

              leaderboardRow.SetNameText(playerData.playerName);
              leaderboardRow.SetMoneyText(playerData.score.ToString());
              leaderboardRow.SetDegreeText(degree.ToString());
              degree++;
            }
          }
        });
    }

    public override void OnRemove()
    {
      view.dispatcher.RemoveListener(LeaderboardEvent.Close, OnCloseTab);
    }
  }
}