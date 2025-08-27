using Features.Input;
using HSM;
using ProjectTools.Ecs;
using Unity.Entities;
using UnityEngine;

namespace Core.States
{
    public class PauseGameState : BaseSubState<PlayingGameState, GameState>
    {
        public override void OnEnter(SystemBase system)
        {
            Debug.Log("Enter PauseGameState");
        }
        
        public override void OnExit(SystemBase system)
        {
            Debug.Log("Exit PauseGameState");
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
    }
}