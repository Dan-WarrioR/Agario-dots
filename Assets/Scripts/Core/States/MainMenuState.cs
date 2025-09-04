using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using ProjectTools.DependencyManagement;
using Unity.Entities;
using UnityEngine;

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
            _screenManager.Open<MainMenuScreen>((screen) =>
            {
                _mainMenuScreen = screen;
                _mainMenuScreen.OnStartGame += OnStartGame;
            });
        }
        
        public override void OnExit(SystemBase system)
        {
            _mainMenuScreen.OnStartGame -= OnStartGame;
            _screenManager.CloseAll();
        }
        
        private void OnStartGame()
        {
            SceneLoadingState.TransitionTo(Parent, new GameState());
        }
    }
}