﻿using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Spawn
{
    public struct Spawner : IComponentData
    {
        public Entity prefab;
        public float3 spawnAnchorPointOffset;
        public float3 spawnZone;
        public float nextSpawnTime;
        public float spawnRate;
        public float maxCount;
    }

    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public float3 spawnAnchorPointOffset;
        public float3 spawnZone;
        public float spawnRate;
        public float maxCount = 100;
        
        private void OnDrawGizmosSelected()
        {
            var color = Color.green;
            color.a = 0.5f;
            Gizmos.color = color;
            var center = transform.position + (Vector3)spawnAnchorPointOffset;
            var size = spawnZone * 2f;

            Gizmos.DrawWireCube(center, size);
            Gizmos.DrawCube(center, size);
        }
    }

    public class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Spawner
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                spawnAnchorPointOffset = authoring.spawnAnchorPointOffset,
                spawnZone = authoring.spawnZone,
                nextSpawnTime = 0.0f,
                spawnRate = authoring.spawnRate,
                maxCount = authoring.maxCount,
            });
        }
    }
}