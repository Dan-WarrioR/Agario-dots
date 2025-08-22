﻿using Data;
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
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial class EatingSystem : SystemBase
    {
        
        private ComponentLookup<EaterTag> _eaterLookup;
        private ComponentLookup<Eatable> _eatableLookup;
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<FactionComponent> _factionLookup;
        
        protected override void OnCreate()
        {
            RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<SimulationSingleton>();
            RequireForUpdate<GameplayConfig>();
            RequireForUpdate<FactionRelationsSingleton>();
            
            _eaterLookup = GetComponentLookup<EaterTag>(true);
            _transformLookup = GetComponentLookup<LocalTransform>(true);
            _eatableLookup = GetComponentLookup<Eatable>(false);
            _factionLookup = GetComponentLookup<FactionComponent>(true);
        }
        
        protected override void OnUpdate()
        {
            var ecbSingleton = World.GetExistingSystemManaged<EndFixedStepSimulationEntityCommandBufferSystem>();
            var ecb = ecbSingleton.CreateCommandBuffer();
            
            _eaterLookup.Update(ref CheckedStateRef);
            _transformLookup.Update(ref CheckedStateRef);
            _eatableLookup.Update(ref CheckedStateRef);
            _factionLookup.Update(ref CheckedStateRef);

            Dependency = new EatingTriggerJob
            {
                gameplayConfig = SystemAPI.GetSingleton<GameplayConfig>(),
                eaterLookup = _eaterLookup,
                transformLookup = _transformLookup,
                eatableLookup = _eatableLookup,
                factionLookup = _factionLookup,
                relationsBlob = SystemAPI.GetSingleton<FactionRelationsSingleton>().blobRef,
                ecb = ecb,
            }.Schedule(
                SystemAPI.GetSingleton<SimulationSingleton>(),
                Dependency
            );
            
            ecbSingleton.AddJobHandleForProducer(Dependency);
        }
    }
    
    [BurstCompile]
    public struct EatingTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public GameplayConfig gameplayConfig;
        [ReadOnly] public ComponentLookup<EaterTag> eaterLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> transformLookup;
        [ReadOnly] public ComponentLookup<FactionComponent> factionLookup;
        [ReadOnly] public BlobAssetReference<FactionRelationsBlob> relationsBlob;
        
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
        
        private bool TryEat(Entity eater, Entity target)
        {
            if (!eatableLookup.TryGetComponent(eater, out var eaterEatable) ||
                !eatableLookup.TryGetComponent(target, out var targetEatable) ||
                !transformLookup.TryGetComponent(eater, out var eaterTransform) ||
                !transformLookup.TryGetComponent(target, out var targetTransform))
            {
                return false;
            }
            
            if (factionLookup.TryGetComponent(eater, out var eaterFaction) &&
                factionLookup.TryGetComponent(target, out var targetFaction))
            {
                if (!FactionUtility.IsEnemy(relationsBlob, eaterFaction.id, targetFaction.id))
                {
                    return false;
                }
            }
            
            float eaterRadius = MassScalerSystem.MassToRadius(eaterEatable.mass, gameplayConfig.massToScaleConversion);
            float distance = math.distance(eaterTransform.Position.xy, targetTransform.Position.xy);
            if (distance > eaterRadius || eaterEatable.mass <= targetEatable.mass)
            {
                return false;
            }
            
            eaterEatable.mass += targetEatable.mass;
            eatableLookup[eater] = eaterEatable;
            ecb.DestroyEntity(target);
            
            return true;
        }
    }
}