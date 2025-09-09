using Features.Controller;
using Features.Faction;
using Unity.Burst;
using Unity.Entities;

namespace Features.Consumption.EatingRules
{
    [BurstCompile]
    public struct EatingContext
    {
        public ComponentLookup<FactionComponent> factionLookup;
        public ComponentLookup<CharacterInstance> characterInstanceLookup;
        public FactionDatabaseComponent relationsBlob;
        
        public void OnCreate(ref SystemState state)
        {
            factionLookup = state.GetComponentLookup<FactionComponent>(true);
            characterInstanceLookup = state.GetComponentLookup<CharacterInstance>(true);
        }

        public void OnUpdate(ref SystemState state, FactionDatabaseComponent relations)
        {
            relationsBlob = relations;
            factionLookup.Update(ref state);
            characterInstanceLookup.Update(ref state);
        }
    }
}