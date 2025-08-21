using Unity.Entities;

namespace Features.Faction
{
    public static class FactionUtility
    {
        public static bool IsFriend(BlobAssetReference<FactionsConfig> config, int factionId, int otherId)
        {
            int index = FindIndex(ref config.Value, factionId);
            if (index < 0)
            {
                return false;
            }

            ref var relations = ref config.Value.Relations[index];
            return Contains(ref relations.Allies, otherId);
        }

        public static bool IsEnemy(BlobAssetReference<FactionsConfig> config, int factionId, int otherId)
        {
            int index = FindIndex(ref config.Value, factionId);
            if (index < 0)
            {
                return false;
            }

            ref var relations = ref config.Value.Relations[index];
            return Contains(ref relations.Enemies, otherId);
        }
        
        private static int FindIndex(ref FactionsConfig config, int factionId)
        {
            for (int i = 0; i < config.Ids.Length; i++)
            {
                if (config.Ids[i] == factionId)
                {
                    return i;
                }
            }
            return -1;
        }
        
        private static bool Contains(ref BlobArray<int> arr, int value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}