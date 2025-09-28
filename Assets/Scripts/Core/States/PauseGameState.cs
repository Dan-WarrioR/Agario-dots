using Features.Input;
using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using ProjectTools.Ecs;
using Unity.Entities;
using UnityEngine;

namespace Core.States
{
    public class PauseGameState : BaseSubState<PlayingGameState, GameState>
    {
        private PauseBaseScreen _pauseBaseScreen;

        public override void OnEnter(SystemBase system)
        {
            _pauseBaseScreen = ScreenAPI.OpenScreen<PauseBaseScreen>(system.World);
            if (_pauseBaseScreen != null)
            {
                _pauseBaseScreen.OnMainMenuReturn += ReturnToMainMenu;
            }
            else
            {
                Debug.LogError($"Null pause screen!");
            }
        }

        public override void OnExit(SystemBase system)
        {
            _pauseBaseScreen.OnMainMenuReturn -= ReturnToMainMenu;
            ScreenAPI.CloseScreen<PauseBaseScreen>(system.World);
            _pauseBaseScreen = null;
        }

        public override void OnUpdate(SystemBase system)
        {
            var bridge = system.GetSingletonEntity<InputBridge>();
            if (!system.EntityManager.GetComponentDataIfEnabled<GameCommands>(bridge, out var input))
            {
                return;
            }
            
            if (input.pause.down)
            {
                SetState(new PlayingGameState());
            }
        }

        private void ReturnToMainMenu()
        {
            Parent.Parent.SetSubState(new SceneLoadingState(MainMenuState.MainMenuSceneName, new MainMenuState()));
        }
    }
}