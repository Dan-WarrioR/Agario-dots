using System;

namespace Features.UI.ScreenManagement.Screens
{
    public class MainMenuBaseScreen : BaseScreen
    {
        public event Action OnStartGame;

        public void Act_StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}