using Unity.Entities;
using UnityEngine;

namespace Features.Faction
{
    public struct FactionComponent : IComponentData
    {
        public int id;
    }
    
    public class FactionAuthoring : MonoBehaviour
    {
        [SerializeField] private FactionDefinitionSO faction;
        
        private class FactionAuthoringBaker : Baker<FactionAuthoring>
        {
            public override void Bake(FactionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FactionComponent
                {
                    id = authoring.faction.Id,
                });
            }
        }
    }
}