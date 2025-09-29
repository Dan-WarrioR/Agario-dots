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
        private PauseScreen _pauseScreen;

        public override void OnEnter(SystemBase system)
        {
            _pauseScreen = ScreenAPI.OpenScreen<PauseScreen>(system.World);
            if (_pauseScreen != null)
            {
                _pauseScreen.OnMainMenuReturn += ReturnToMainMenu;
            }
            else
            {
                Debug.LogError($"Null pause screen!");
            }
        }

        public override void OnExit(SystemBase system)
        {
            _pauseScreen.OnMainMenuReturn -= ReturnToMainMenu;
            ScreenAPI.CloseScreen<PauseScreen>(system.World);
            _pauseScreen = null;
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
            Parent.Parent.SetSubState(new MainMenuState());
        }
    }
}