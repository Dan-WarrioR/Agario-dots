using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Features.Faction
{
    public struct FactionBlobAsset
    {
        public BlobArray<FactionEntryBlob> entries;
    }
    
    public struct FactionEntryBlob
    {
        public int id;
        public Color color;
        
        public BlobArray<byte> alliesBitset;
        public BlobArray<byte> enemiesBitset;
    }
    
    public struct FactionDatabaseComponent : IComponentData
    {
        public BlobAssetReference<FactionBlobAsset> blob;
    }
    
    public class FactionRelationAuthoring : MonoBehaviour
    {
        [SerializeField] private FactionDefinitionSO[] factions;
        
        private class FactionRelationAuthoringBaker : Baker<FactionRelationAuthoring>
        {
            public override void Bake(FactionRelationAuthoring authoring)
            {
                int count = authoring.factions.Length;
                if (count == 0)
                {
                    return;
                }

                using var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<FactionBlobAsset>();
                var entries = builder.Allocate(ref root.entries, count);
                int bitsetSize = (count + 7) / 8;

                for (int i = 0; i < count; i++)
                {
                    var definition = authoring.factions[i];
                    ref var entry = ref entries[definition.Id];
                    entry.id = definition.Id;
                    entry.color = definition.Color;
                    var allies = builder.Allocate(ref entry.alliesBitset, bitsetSize);
                    var enemies = builder.Allocate(ref entry.enemiesBitset, bitsetSize);

                    for (int b = 0; b < bitsetSize; b++)
                    {
                        allies[b] = 0;
                        enemies[b] = 0;
                    }
                    
                    FillBitset(ref allies, definition.Allies);
                    FillBitset(ref enemies, definition.Enemies);
                }

                var blob = builder.CreateBlobAssetReference<FactionBlobAsset>(Allocator.Persistent);
                AddBlobAsset(ref blob, out _);

                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FactionDatabaseComponent { blob = blob });
            }
            
            private static void FillBitset(ref BlobBuilderArray<byte> bitset, FactionDefinitionSO[] factions)
            {
                foreach (var definition in factions)
                {
                    int idx = definition.Id >> 3;
                    int mask = 1 << (definition.Id & 7);
                    bitset[idx] |= (byte)mask;
                }
            }
        }
    }
}