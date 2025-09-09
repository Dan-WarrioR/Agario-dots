using Features.Faction;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

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

            if (FactionUtility.IsEnemy(ref ctx.relationsBlob.blob.Value, eaterFaction.id, targetFaction.id))
            {
                Debug.Log("ENEMY");
                return true;
            }
            else
            {
                Debug.Log($"Not enemy! Parents == ? {targetCharacterInstance.parent == eaterCharacterInstance.parent}");
            }

            return targetCharacterInstance.parent == eaterCharacterInstance.parent;
        }
    }
}