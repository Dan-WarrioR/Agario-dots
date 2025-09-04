using Unity.Entities;

namespace Features.Faction
{
    public static class FactionUtility
    {
        public static bool IsFriend(BlobAssetReference<FactionRelationsBlob> blobRef, int selfID, int targetID)
        {
            if (selfID == targetID)
            {
                return true;
            }
            
            ref var relations = ref GetRelationsByID(blobRef, selfID);
            
            return Contains(ref relations.allies, targetID);
        }

        public static bool IsEnemy(BlobAssetReference<FactionRelationsBlob> blobRef, int selfID, int targetID)
        {
            ref var relations = ref GetRelationsByID(blobRef, selfID);
            
            return Contains(ref relations.enemies, targetID);
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
        
        private static ref FactionRelations GetRelationsByID(in BlobAssetReference<FactionRelationsBlob> blobRef, int id)
        {
            ref var entries = ref blobRef.Value.entries;
            int low = 0, high = entries.Length - 1;

            while (low <= high)
            {
                int mid = (low + high) >> 1;
                int midId = entries[mid].id;

                if (midId < id)
                {
                    low = mid + 1;
                }
                else if (midId > id)
                {
                    high = mid - 1;
                }
                else
                {
                    return ref entries[mid];
                }
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogError($"Faction with id {id} not found in blob");
#endif
            return ref entries[0];
        }
    }
}