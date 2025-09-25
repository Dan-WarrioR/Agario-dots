using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using Unity.Entities;
using UnityEngine;

namespace Core.States
{
    public class MainMenuState : BaseSubState<MainMenuState, AppHsm>, ILoadableState
    {
        private const string MainMenuSceneName = "MainMenu";
        
        public SceneLoadingState.Data Data => new(MainMenuSceneName, this);
        
        private MainMenuBaseScreen _mainMenuBaseScreen;

        public override void OnEnter(SystemBase system)
        {
            _mainMenuBaseScreen = ScreenAPI.OpenScreen<MainMenuBaseScreen>(system.World);
            if (_mainMenuBaseScreen != null)
            {
                _mainMenuBaseScreen.OnStartGame += OnStartGame;
            }
            else
            {
                Debug.LogError($"Null pause screen!");
            }
        }
        
        public override void OnExit(SystemBase system)
        {
            _mainMenuBaseScreen.OnStartGame -= OnStartGame;
            ScreenAPI.CloseScreen<MainMenuBaseScreen>(system.World);
            _mainMenuBaseScreen = null;
        }
        
        private void OnStartGame()
        {
            SceneLoadingState.TransitionTo(Parent, new GameState());
        }
    }
}