using Data;
using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using ProjectTools.DependencyManagement;
using Unity.Entities;

namespace Core.States
{
    public class MainMenuState : BaseSubState<MainMenuState, AppHsm>, ILoadableState
    {
        private const string MainMenuSceneName = "MainMenu";
        
        public SceneLoadingState.Data Data => new(MainMenuSceneName, this);
        
        private MainMenuScreen _mainMenuScreen;
        private ScreenManager _screenManager;

        public override void OnEnter(SystemBase system)
        {
            _screenManager = Service.Get<ScreenManager>();
            var screenPrefab = Service.Get<UIConfig>().mainMenuScreen;
            
            _mainMenuScreen = _screenManager.Open(screenPrefab);
            _mainMenuScreen.OnStartGame += OnStartGame;
        }
        
        public override void OnExit(SystemBase system)
        {
            _screenManager.Close(_mainMenuScreen);
            _mainMenuScreen.OnStartGame -= OnStartGame;
        }
        
        private void OnStartGame()
        {
            SceneLoadingState.TransitionTo(Parent, new GameState());
        }
    }
}