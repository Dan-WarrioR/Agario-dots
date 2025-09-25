using ProjectTools.Ecs;
using Unity.Collections;
using Unity.Entities;

namespace Features.UI.ScreenManagement.Screens
{
    public struct StatisticsSingleton : IComponentData {}

    public struct CountPerLayer : IBufferElementData
    {
        public int layerId;
        public int count;
    }
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct StatisticSystem : ISystem
    {
        private Entity _singleton;
        private NativeParallelHashMap<int, int> _map;

        public void OnCreate(ref SystemState state)
        {
            _singleton = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(_singleton, new StatisticsSingleton());
            state.EntityManager.AddBuffer<CountPerLayer>(_singleton);

            _map = new NativeParallelHashMap<int,int>(16, Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            _map.Clear();
            var buffer = state.EntityManager.GetBuffer<CountPerLayer>(_singleton);
            buffer.Clear();

            foreach (var members in SystemAPI.Query<DynamicBuffer<LayerMember>>())
            {
                for (int i = 0; i < members.Length; i++)
                {
                    int id = members[i].layerId;
                    if (_map.TryGetValue(id, out var c))
                    {
                        _map[id] = c + 1;
                    }
                    else
                    {
                        _map[id] = 1;
                    }
                }
            }

            foreach (var kv in _map)
            {
                buffer.Add(new CountPerLayer { layerId = kv.Key, count = kv.Value });
            }
        }
        
        public void OnDestroy(ref SystemState state)
        {
            if (_map.IsCreated)
            {
                _map.Dispose();
            }
        }
    }
}