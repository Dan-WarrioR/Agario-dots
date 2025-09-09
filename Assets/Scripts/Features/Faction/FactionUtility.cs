using System.Runtime.CompilerServices;
using Unity.Entities;
using UnityEngine;

namespace Features.Faction
{
    public static class FactionUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnemy(ref FactionBlobAsset db, int factionA, int factionB)
        {
            return factionA != factionB && Contains(ref db.entries[factionA].enemiesBitset, factionB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFriend(ref FactionBlobAsset db, int factionA, int factionB)
        {
            return factionA == factionB || Contains(ref db.entries[factionA].alliesBitset, factionB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetColor(ref FactionBlobAsset db, int factionId)
        {
            return db.entries[factionId].color;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Contains(ref BlobArray<byte> bitset, int factionId)
        {
            int idx = factionId >> 3;
            int mask = 1 << (factionId & 7);
            return (bitset[idx] & mask) != 0;
        }
    }
}