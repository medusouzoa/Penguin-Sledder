using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Context.Scripts.View.Leaderboard
{
  public class LeaderboardView : EventView
  {
    public Transform container;
    public GameObject rowSlot;
    public Transform loginContainer;
    public GameObject loginSlot;
    
    public void OnCloseTab()
    {
      dispatcher.Dispatch(LeaderboardEvent.Close);
    }

  }
}