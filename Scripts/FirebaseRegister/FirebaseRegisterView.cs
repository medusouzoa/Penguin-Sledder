using Runtime.Context.Scripts.View.FirebaseAuth;
using strange.extensions.mediation.impl;
using TMPro;

namespace Runtime.Context.Scripts.View.FirebaseRegister
{
  public class FirebaseRegisterView : EventView
  {
    public TMP_InputField fullName;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_InputField passwordConfirmationField;

    public void OnRegisterGame()
    {
      dispatcher.Dispatch(AuthStateEvent.Register);
    }

    public void OnLogInGame()
    {
      dispatcher.Dispatch(AuthStateEvent.LogIn);
    }

    public void OnBackToMainMenu()
    {
      dispatcher.Dispatch(AuthStateEvent.MainMenu);
    }

    public void OnRegisterWithGoogle()
    {
      dispatcher.Dispatch(AuthStateEvent.GoogleRegistry);
    }
  }
}