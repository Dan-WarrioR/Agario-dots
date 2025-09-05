using Features.Faction;
using Unity.Burst;
using Unity.Entities;

namespace Features.Consumption.EatingRules
{
    [BurstCompile]
    public struct FactionEatingRule
    {
        [BurstCompile]
        public static bool TryEat(in Entity eater, in Entity target, ref EatingContext ctx)
        {
            if (!ctx.factionLookup.TryGetComponent(eater, out var eaterFaction) 
                || !ctx.factionLookup.TryGetComponent(target, out var targetFaction)
                || !ctx.characterInstanceLookup.TryGetComponent(eater, out var targetCharacterInstance)
                || !ctx.characterInstanceLookup.TryGetComponent(target, out var eaterCharacterInstance))
            {
                return true;
            }

            if (FactionUtility.IsEnemy(ctx.relationsBlob, eaterFaction.id, targetFaction.id))
            {
                return true;
            }

            return targetCharacterInstance.parent == eaterCharacterInstance.parent;
        }
    }
}