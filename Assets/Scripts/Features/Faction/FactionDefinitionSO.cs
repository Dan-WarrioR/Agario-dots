using UnityEngine;

namespace Features.Faction
{
    [CreateAssetMenu(menuName = "Data/FactionDefinitionSO")]
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