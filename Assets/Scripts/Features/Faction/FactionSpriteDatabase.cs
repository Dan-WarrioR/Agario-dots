using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Features.Faction
{
    public class FactionSpriteDatabase : IComponentData
    {
        public static FactionSpriteDatabase Instance => _instance ??= new FactionSpriteDatabase();
        
        private static FactionSpriteDatabase _instance;
        private Dictionary<int, Sprite> _spritesByFactionId = new Dictionary<int, Sprite>();

        public void AddSprite(int factionId, Sprite sprite)
        {
            if (sprite != null)
            {
                _spritesByFactionId[factionId] = sprite;
            }
        }

        public Sprite GetSprite(int factionId)
        {
            if (_spritesByFactionId.TryGetValue(factionId, out var sprite))
            {
                return sprite;
            }
            return null;
        }
    }
}
