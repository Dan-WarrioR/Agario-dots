using System;
using Data;
using Features.Consumption.EatingRules;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Features.Faction;

namespace Features.Consumption
{
    public delegate bool TryEatRuleDelegate(in Entity eater, in Entity target, ref EatingContext context);
    
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial class EatingSystem : SystemBase
    {
        private ComponentLookup<EaterTag> _eaterLookup;
        private ComponentLookup<Eatable> _eatableLookup;
        private ComponentLookup<LocalTransform> _transformLookup;
        
        private EatingContext _context;
        private NativeList<FunctionPointer<TryEatRuleDelegate>> _rules;
        
        protected override void OnCreate()
        {
            RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<SimulationSingleton>();
            RequireForUpdate<GameplayConfig>();
            RequireForUpdate<FactionRelationsSingleton>();
            
            _eaterLookup = GetComponentLookup<EaterTag>(true);
            _transformLookup = GetComponentLookup<LocalTransform>(true);
            _eatableLookup = GetComponentLookup<Eatable>(false);
            
            _context = new();
            _context.OnCreate(ref CheckedStateRef);
            
            var rulesCount = Enum.GetValues(typeof(EatingRuleType)).Length;
            _rules = new(rulesCount, Allocator.Persistent);
        }
        
        protected override void OnUpdate()
        {
            var ecbSingleton = World.GetExistingSystemManaged<EndFixedStepSimulationEntityCommandBufferSystem>();
            var ecb = ecbSingleton.CreateCommandBuffer();
            
            _eaterLookup.Update(ref CheckedStateRef);
            _transformLookup.Update(ref CheckedStateRef);
            _eatableLookup.Update(ref CheckedStateRef);
            _context.OnUpdate(ref CheckedStateRef, 
                SystemAPI.GetSingleton<FactionRelationsSingleton>().blobRef);

            Dependency = new EatingTriggerJob
            {
                gameplayConfig = SystemAPI.GetSingleton<GameplayConfig>(),
                eaterLookup = _eaterLookup,
                transformLookup = _transformLookup,
                eatableLookup = _eatableLookup,
                eatingContext = _context,
                rules = _rules,
                ecb = ecb,
            }.Schedule(
                SystemAPI.GetSingleton<SimulationSingleton>(),
                Dependency
            );
            ecbSingleton.AddJobHandleForProducer(Dependency);
        }
        
        protected override void OnDestroy()
        {
            if (_rules.IsCreated)
            {
                _rules.Dispose();
            }
        }
        
        public void AddRule(TryEatRuleDelegate rule)
        {
            _rules.Add(BurstCompiler.CompileFunctionPointer(rule));
        }
    }
    
    [BurstCompile]
    public struct EatingTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public GameplayConfig gameplayConfig;
        [ReadOnly] public ComponentLookup<EaterTag> eaterLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> transformLookup;
        
        [ReadOnly] public EatingContext eatingContext;
        [ReadOnly] public NativeList<FunctionPointer<TryEatRuleDelegate>> rules;
        
        [NativeDisableParallelForRestriction]
        public ComponentLookup<Eatable> eatableLookup;
        public EntityCommandBuffer ecb;
    
        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;
        
            if (eaterLookup.HasComponent(entityA) && TryEat(entityA, entityB))
            {
                return;
            }
        
            if (eaterLookup.HasComponent(entityB))
            {
                TryEat(entityB, entityA);
            }
        }
        
        private bool TryEat(in Entity eater, in Entity target)
        {
            if (!eatableLookup.TryGetComponent(eater, out var eaterEatable) ||
                !eatableLookup.TryGetComponent(target, out var targetEatable) ||
                !transformLookup.TryGetComponent(eater, out var eaterTransform) ||
                !transformLookup.TryGetComponent(target, out var targetTransform))
            {
                return false;
            }
            
            float eaterRadius = MassScalerSystem.MassToRadius(eaterEatable.mass, gameplayConfig.massToScaleConversion);
            float distance = math.distance(eaterTransform.Position.xy, targetTransform.Position.xy);
            if (distance > eaterRadius || eaterEatable.mass <= targetEatable.mass)
            {
                return false;
            }
            
            foreach (var kvp in rules)
            {
                if (!kvp.Invoke(eater, target, ref eatingContext))
                {
                    return false;
                }
            }
            
            eaterEatable.mass += targetEatable.mass;
            eatableLookup[eater] = eaterEatable;
            ecb.DestroyEntity(target);
            
            return true;
        }
    }
}