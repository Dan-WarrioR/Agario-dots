using System;

namespace Features.UI.ScreenManagement.Screens
{
    public class MainMenuScreen : BaseScreen
    {
        public event Action OnStartGame;

        public void Act_StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}