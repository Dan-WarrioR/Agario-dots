using Features.Input;
using HSM;
using ProjectTools.Ecs;
using Unity.Entities;

namespace Core.States
{
    public class PauseGameState : BaseSubState<PlayingGameState, GameState>
    {
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
    }
}