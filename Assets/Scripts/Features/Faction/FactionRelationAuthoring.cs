using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Features.Faction
{
   
    public struct FactionsConfig
    {
        public BlobArray<FactionRelations> Relations;
        public BlobArray<int> Ids;
    }

    public struct FactionRelations
    {
        public BlobArray<int> Allies;
        public BlobArray<int> Enemies;
    }

    public struct FactionsConfigSingleton : IComponentData
    {
        public BlobAssetReference<FactionsConfig> Blob;
    }
    
    public class FactionRelationAuthoring : MonoBehaviour
    {
        [SerializeField] private FactionDefinitionSO[] factions;
        
        private class FactionRelationAuthoringBaker : Baker<FactionRelationAuthoring>
        {
            public override void Bake(FactionRelationAuthoring authoring)
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<FactionsConfig>();

                var relationsArray = builder.Allocate(ref root.Relations, authoring.factions.Length);
                var idsArray = builder.Allocate(ref root.Ids, authoring.factions.Length);

                for (int f = 0; f < authoring.factions.Length; f++)
                {
                    var faction = authoring.factions[f];
                    idsArray[f] = faction.Id;

                    var alliesArray = builder.Allocate(ref relationsArray[f].Allies, faction.Allies.Length);
                    for (int i = 0; i < faction.Allies.Length; i++)
                    {
                        alliesArray[i] = faction.Allies[i].Id;
                    }

                    var enemiesArray = builder.Allocate(ref relationsArray[f].Enemies, faction.Enemies.Length);
                    for (int i = 0; i < faction.Enemies.Length; i++)
                    {
                        enemiesArray[i] = faction.Enemies[i].Id;
                    }
                }

                var blobRef = builder.CreateBlobAssetReference<FactionsConfig>(Allocator.Persistent);
                builder.Dispose();

                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FactionsConfigSingleton { Blob = blobRef });
            }
        }
    }
}