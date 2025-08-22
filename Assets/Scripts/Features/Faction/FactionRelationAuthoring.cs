using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Features.Faction
{
    public struct FactionRelationsBlob
    {
        public BlobArray<FactionRelations> entries;
    }
    
    public struct FactionRelations
    {
        public int id;
        public BlobArray<int> allies;
        public BlobArray<int> enemies;
    }
    
    public struct FactionRelationsSingleton : IComponentData
    {
        public BlobAssetReference<FactionRelationsBlob> blobRef;
    }
    
    public class FactionRelationAuthoring : MonoBehaviour
    {
        [SerializeField] private FactionDefinitionSO[] factions;
        
        private class FactionRelationAuthoringBaker : Baker<FactionRelationAuthoring>
        {
            public override void Bake(FactionRelationAuthoring authoring)
            {
                using var builder = new BlobBuilder(Allocator.Temp);

                ref var root = ref builder.ConstructRoot<FactionRelationsBlob>();
                var entries = builder.Allocate(ref root.entries, authoring.factions.Length);

                var sorted = authoring.factions.OrderBy(f => f.Id).ToArray();

                for (int i = 0; i < sorted.Length; i++)
                {
                    entries[i].id = sorted[i].Id;

                    var allies = builder.Allocate(ref entries[i].allies, sorted[i].Allies.Length);
                    for (int j = 0; j < sorted[i].Allies.Length; j++)
                    {
                        allies[j] = sorted[i].Allies[j].Id;
                    }

                    var enemies = builder.Allocate(ref entries[i].enemies, sorted[i].Enemies.Length);
                    for (int j = 0; j < sorted[i].Enemies.Length; j++)
                    {
                        enemies[j] = sorted[i].Enemies[j].Id;
                    }
                }

                var blobRef = builder.CreateBlobAssetReference<FactionRelationsBlob>(Allocator.Persistent);
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FactionRelationsSingleton
                {
                    blobRef = blobRef,
                });
            }
        }
    }
}