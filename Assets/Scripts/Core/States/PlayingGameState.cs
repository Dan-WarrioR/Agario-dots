using System;
using System.Collections.Generic;
using Features;
using Features.Controller;
using Features.Movement;
using HSM;
using Unity.Entities;
using Features.Input;
using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using ProjectTools.Ecs;

namespace Core.States
{
    public class PlayingGameState : BaseSubState<PlayingGameState, GameState>
    {
        protected override List<Type> RequiredSystems => new()
        {
            typeof(MotionSystem),
            typeof(CharacterControllerSystem),
            typeof(GameplayGroup),
        };

        public override void OnEnter(SystemBase system)
        {
            ScreenAPI.OpenScreen<StatisticScreen>(system.World);
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
                SetState(new PauseGameState());
            }
        }
    }
}