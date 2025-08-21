using UnityEngine;

namespace Features.Faction
{
    [CreateAssetMenu(fileName = "FactionComponent", menuName = "Data/FactionComponent")]
    public class FactionDefinitionSO : ScriptableObject
    {
        [SerializeField] private int id;
        [SerializeField] private FactionDefinitionSO[] allies;
        [SerializeField] private FactionDefinitionSO[] enemies;
        
        public int Id => id;
        public FactionDefinitionSO[] Allies => allies;
        public FactionDefinitionSO[] Enemies => enemies;
    }
}