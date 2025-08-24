using Unity.Burst;
using Unity.Entities;
using ProjectTools.Ecs;

namespace Features.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(InputSystem))]
    public partial struct InputEventsCleanupSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputBridge>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entity = SystemAPI.GetSingletonEntity<InputBridge>();
            if (!state.EntityManager.GetComponentDataIfEnabled<GameCommands>(entity, out var commands))
            {
                return;
            }

            commands.ResetFrameEvents();
            state.EntityManager.SetComponentData(entity, commands);
        }
    }
}