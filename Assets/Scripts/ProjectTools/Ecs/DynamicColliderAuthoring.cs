﻿using System;
using ProjectTools.Ecs.DynamicColliders;
using Unity.Entities;
using UnityEngine;

namespace ProjectTools.Ecs
{
    public struct LayerMember : IComponentData
    {
        public int layerId;
    }
    
    [InternalBufferCapacity(5)]
    public struct DynamicForcedCollision : IBufferElementData
    {
        public uint withLayer;
        public double cooldown;
    }
    
    public readonly partial struct DynamicCollisionAspect : IAspect
    {
        public readonly RefRW<LayerMember> dynamicLayer;
        
        public readonly DynamicBuffer<DynamicForcedCollision> allowedCollisions;
    }

    public class DynamicColliderAuthoring : MonoBehaviour
    {
        [Serializable]
        public class ForcedCollision
        {
            public uint withLayer;
            public double cooldown = -1;
        }
        
        public LayerDefinitionSO layerId;
        public ForcedCollision[] forcedCollisions;
    }
    
    public class DynamicColliderBaker : Baker<DynamicColliderAuthoring>
    {
        public override void Bake(DynamicColliderAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new LayerMember
            {
                layerId = authoring.layerId.Id,
            });

            AddBuffer<DynamicForcedCollision>(entity);
            if (authoring.forcedCollisions != null)
            {
                foreach (var forced in authoring.forcedCollisions)
                {
                    AppendToBuffer(entity, new DynamicForcedCollision
                    {
                        withLayer = forced.withLayer,
                        cooldown = forced.cooldown,
                    });
                }
            }
        }
    }
}