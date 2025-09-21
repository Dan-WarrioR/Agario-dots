using Unity.Entities;
using UnityEngine;

namespace ProjectTools.Ecs.DynamicColliders
{
    public static class LayerUtility
    {
        public static bool IsInteracting(ref LayerBlobAsset db, int layerA, int layerB)
        {
            return Contains(ref db.entries[layerA].interactBitset, layerB);
        }

        public static Color GetColor(ref LayerBlobAsset db, int layerId)
        {
            return db.entries[layerId].color;
        }

        private static bool Contains(ref BlobArray<byte> bitset, int id)
        {
            int idx = id >> 3;
            int mask = 1 << (id & 7);
            return (bitset[idx] & mask) != 0;
        }
    }
}