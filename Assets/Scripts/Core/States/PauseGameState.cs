using Features.Input;
using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using ProjectTools.DependencyManagement;
using ProjectTools.Ecs;
using Unity.Entities;

namespace Core.States
{
    public class PauseGameState : BaseSubState<PlayingGameState, GameState>
    {
        private PauseScreen _pauseScreen;
        private ScreenManager _screenManager;

        public override void OnEnter(SystemBase system)
        {
            _screenManager = Service.Get<ScreenManager>();
            _screenManager.Open<PauseScreen>((screen) =>
            {
                _pauseScreen = screen;
                _pauseScreen.OnMainMenuReturn += ReturnToMainMenu;
            });
        }

        public override void OnExit(SystemBase system)
        {
            _pauseScreen.OnMainMenuReturn -= ReturnToMainMenu;
            _screenManager.CloseAll();
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
            SceneLoadingState.TransitionTo(Parent.Parent, new MainMenuState());
        }
    }
}