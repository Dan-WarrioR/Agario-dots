using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Features.Faction
{
    public class FactionBehaviorDatabase : IComponentData
    {
        public static FactionBehaviorDatabase Instance => _instance ??= new FactionBehaviorDatabase();
        
        private static FactionBehaviorDatabase _instance;
        private Dictionary<int, ScriptableObject> _behaviorsByFactionId = new Dictionary<int, ScriptableObject>();

        public void AddBehavior(int factionId, ScriptableObject behavior)
        {
            if (behavior != null)
            {
                _behaviorsByFactionId[factionId] = behavior;
            }
        }
    }
}
