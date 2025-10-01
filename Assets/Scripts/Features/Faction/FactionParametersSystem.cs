using ProjectTools.Ecs;
using ProjectTools.Ecs.DynamicColliders;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Features.Faction
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial class FactionParametersSystem : SystemBase
    {
        private ComponentLookup<LayerMember> _factionLookup;
        private readonly Color _standardFactionColor = Color.black;

        protected override void OnCreate()
        {
            RequireForUpdate<LayerDatabaseComponent>();
            RequireForUpdate<LayerParametersInitialTag>();

            _factionLookup = GetComponentLookup<LayerMember>(true);
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton<LayerDatabaseComponent>(out var factionDatabase)) return;

            _factionLookup.Update(ref CheckedStateRef);

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var factionLookup = _factionLookup;
            Entities
                .WithAll<LayerParametersInitialTag>()
                .WithReadOnly(factionLookup)
                .ForEach((Entity entity) =>
                {
                    if (!factionLookup.TryGetComponent(entity, out var faction)) return;

                    var factionColor = LayerUtility.GetColor(ref factionDatabase.blob.Value, (int)faction.layerId);
                    var factionSprite = FactionSpriteDatabase.Instance?.GetSprite(faction.layerId);

                    UpdateSpriteRenderer(entity, factionColor, factionSprite);

                    ecb.RemoveComponent<LayerParametersInitialTag>(entity);
                })
                .WithoutBurst()
                .Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }


        /*
        private Color GetFactionColor(ref LayerBlobAsset db, uint factionId)
        {
            for (int i = 0; i < db.entries.Length; i++)
            {
                if (db.entries[i].id == factionId)
                {
                    var color = db.entries[i].color;
                    return new Color(color.x, color.y, color.z, color.w);
                }
            }

            return _standardFactionColor;
        }*/

        private void UpdateSpriteRenderer(Entity entity, Color color, Sprite sprite)
        {
            var entityManager = World.EntityManager;

            if (entityManager.HasComponent<SpriteRenderer>(entity))
            {
                var spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
                if (spriteRenderer != null)
                {
                    if (color != default)
                    {
                        spriteRenderer.color = color;
                    }

                    if (sprite != null)
                    {
                        spriteRenderer.sprite = sprite;
                    }
                }
            }
        }
    }
}