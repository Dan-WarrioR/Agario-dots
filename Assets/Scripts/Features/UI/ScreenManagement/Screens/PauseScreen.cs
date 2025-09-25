using System;

namespace Features.UI.ScreenManagement.Screens
{
    public class PauseBaseScreen : BaseScreen
    {
        public event Action OnMainMenuReturn;
        
        public void Act_ReturnToMainMenu()
        {
            OnMainMenuReturn?.Invoke();
        }
    }
}