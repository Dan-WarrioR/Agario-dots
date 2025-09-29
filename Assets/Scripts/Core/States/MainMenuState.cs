using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using Unity.Entities;

namespace Core.States
{
    public class MainMenuState : SceneState<MainMenuState, AppHsm>
    {
        protected override string SceneName => "MainMenu";
        
        private MainMenuScreen _mainMenuScreen;
        
        protected override void OnSceneEnter(SystemBase system)
        {
            _mainMenuScreen = ScreenAPI.OpenScreen<MainMenuScreen>(system.World);
            _mainMenuScreen.OnStartGame += OnStartGame;
        }

        public override void OnExit(SystemBase system)
        {
            if (_mainMenuScreen != null)
            {
                _mainMenuScreen.OnStartGame -= OnStartGame;
                ScreenAPI.CloseScreen<MainMenuScreen>(system.World);
                _mainMenuScreen = null;
            }
        }

        private void OnStartGame()
        {
            Parent.SetSubState(new GameState());
        }
    }
}