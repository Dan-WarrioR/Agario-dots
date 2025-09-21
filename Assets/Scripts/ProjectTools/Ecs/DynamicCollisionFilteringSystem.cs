using ProjectTools.Ecs.DynamicColliders;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace ProjectTools.Ecs
{
    [UpdateInGroup(typeof(PhysicsCreateContactsGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct DynamicCollisionFilteringSystem : ISystem
    {
        private BufferLookup<LayerMember> _layerMemberLookup;
        private BufferLookup<DynamicForcedCollision> _dynamicCollisionLookup;
        private ComponentLookup<PhysicsCollider> _physicsColliderLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<LayerDatabaseComponent>();

            _layerMemberLookup = state.GetBufferLookup<LayerMember>(true);
            _dynamicCollisionLookup = state.GetBufferLookup<DynamicForcedCollision>();
            _physicsColliderLookup = state.GetComponentLookup<PhysicsCollider>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            _layerMemberLookup.Update(ref state);
            _dynamicCollisionLookup.Update(ref state);
            _physicsColliderLookup.Update(ref state);

            state.Dependency = new CollisionOverrideJob
            {
                layerMemberLookup = _layerMemberLookup,
                dynamicCollisionLookup = _dynamicCollisionLookup,
                physicsColliderLookup = _physicsColliderLookup,
                layerDb = SystemAPI.GetSingleton<LayerDatabaseComponent>().blob,
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), ref physicsWorld, state.Dependency);
        }
    }
    
    
    [BurstCompile]
    public struct CollisionOverrideJob : IContactsJob
    {
        [ReadOnly] public BufferLookup<LayerMember> layerMemberLookup;
        [ReadOnly] public BufferLookup<DynamicForcedCollision> dynamicCollisionLookup;
        [ReadOnly] public ComponentLookup<PhysicsCollider> physicsColliderLookup;
        [ReadOnly] public BlobAssetReference<LayerBlobAsset> layerDb;

        [BurstCompile]
        public void Execute(ref ModifiableContactHeader header, ref ModifiableContactPoint contact)
        {
            var entityA = header.EntityA;
            var entityB = header.EntityB;

            if ((header.JacobianFlags &= JacobianFlags.IsTrigger) == 0)
            {
                return;
            }

            if (IsColliding(entityA, entityB) || IsColliding(entityB, entityA))
            {
                header.JacobianFlags |= JacobianFlags.EnableCollisionEvents;
            }
        }
        
        private bool IsColliding(in Entity entityA, in Entity entityB)
        {
            if (!layerMemberLookup.TryGetBuffer(entityA, out var layersA)
                || !layerMemberLookup.TryGetBuffer(entityB, out var layersB))
            {
                return false;
            }

            if (dynamicCollisionLookup.TryGetBuffer(entityB, out var forcedBuffer))
            {
                foreach (var forced in forcedBuffer)
                {
                    foreach (var la in layersA)
                    {
                        if (la.layerId == forced.withLayer)
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var la in layersA)
            {
                foreach (var lb in layersB)
                {
                    if (LayerUtility.IsInteracting(ref layerDb.Value, la.layerId, lb.layerId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}