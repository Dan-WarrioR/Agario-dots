using System.Collections.Generic;
using ProjectTools.Ecs;
using ProjectTools.Ecs.DynamicColliders;
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
        private struct DescendingComparer : IComparer<int>
        {
            public int Compare(int a, int b)
            {
                return b.CompareTo(a);
            }
        }
        
        private Entity _singleton;
        private NativeList<int> _counters;
        private DescendingComparer _comparer;
        
        private EntityQuery _layerQuery;
        private int _lastTotalCount;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LayerDatabaseComponent>();
            state.RequireForUpdate<LayerMember>();
            
            _singleton = state.EntityManager.CreateEntity();
            _comparer = new();
            state.EntityManager.AddComponentData(_singleton, new StatisticsSingleton());
            state.EntityManager.AddBuffer<CountPerLayer>(_singleton);

            _layerQuery = state.GetEntityQuery(typeof(LayerMember));
            _lastTotalCount = -1;
        }
        
        public void OnUpdate(ref SystemState state)
        {
            int currentTotal = _layerQuery.CalculateEntityCount();
            
            if (_lastTotalCount == currentTotal)
            {
                return;
            }
            
            _lastTotalCount = currentTotal;

            InitializeCounters();
            
            for (int i = 0; i < _counters.Length; i++)
            {
                _counters[i] = 0;
            }
            
            foreach (var memberRO in SystemAPI.Query<RefRO<LayerMember>>())
            {
                int id = memberRO.ValueRO.layerId;
                _counters[id] += 1;
            }
            
            _counters.Sort(_comparer);

            var buffer = state.EntityManager.GetBuffer<CountPerLayer>(_singleton);
            buffer.Clear();
            buffer.Capacity = _counters.Length;
            for (int i = 0; i < _counters.Length; i++)
            {
                buffer.Add(new CountPerLayer {layerId = i, count = _counters[i]});
            }
        }
        
        public void OnDestroy(ref SystemState state)
        {
            if (_counters.IsCreated)
            {
                _counters.Dispose();
            }
        }

        private void InitializeCounters()
        {
            if (_counters.IsCreated 
                || !SystemAPI.TryGetSingleton(out LayerDatabaseComponent layerDatabase))
            {
                return;
            }
            
            int count = layerDatabase.blob.Value.entries.Length;
            
            _counters = new NativeList<int>(count, Allocator.Persistent);
            for (int i = 0; i < count; i++)
            {
                _counters.Add(0);
            }
        }
    }
}