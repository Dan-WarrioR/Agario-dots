using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ProjectTools.Ecs.DynamicColliders
{
    public struct LayerBlobAsset
    {
        public BlobArray<LayerEntryBlob> entries;
    }

    public struct LayerEntryBlob
    {
        public int id;
        public Color color;
        public BlobArray<byte> interactBitset;
    }

    public struct LayerDatabaseComponent : IComponentData
    {
        public BlobAssetReference<LayerBlobAsset> blob;
    }
    
    public class LayerRelationAuthoring : MonoBehaviour
    {
        [SerializeField] private LayerDefinitionSO[] layers;

        private class LayerRelationAuthoringBaker : Baker<LayerRelationAuthoring>
        {
            public override void Bake(LayerRelationAuthoring authoring)
            {
                int count = authoring.layers.Length;
                if (count == 0)
                {
                    return;
                }
                
                using var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<LayerBlobAsset>();
                var entries = builder.Allocate(ref root.entries, count);
                int bitsetSize = (count + 7) / 8;

                for (int i = 0; i < count; i++)
                {
                    var definition = authoring.layers[i];
                    ref var entry = ref entries[i];
                    entry.id = definition.Id;
                    entry.color = definition.Color;
                    var interact = builder.Allocate(ref entry.interactBitset, bitsetSize);

                    for (int b = 0; b < bitsetSize; b++)
                    {
                        interact[b] = 0;
                    }

                    FillBitset(ref interact, definition.InteractsWith);
                }

                var blob = builder.CreateBlobAssetReference<LayerBlobAsset>(Allocator.Persistent);
                AddBlobAsset(ref blob, out _);

                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new LayerDatabaseComponent {blob = blob});
            }

            private static void FillBitset(ref BlobBuilderArray<byte> bitset, LayerDefinitionSO[] layers)
            {
                foreach (var definition in layers)
                {
                    int idx = definition.Id >> 3;
                    int mask = 1 << (definition.Id & 7);
                    bitset[idx] |= (byte) mask;
                }
            }
        }
    }
}