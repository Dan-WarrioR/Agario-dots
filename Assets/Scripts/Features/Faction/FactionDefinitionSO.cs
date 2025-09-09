using Unity.Collections;
using UnityEngine;

namespace Features.Faction
{
    [CreateAssetMenu(menuName = "Data/FactionDefinitionSO")]
    public class FactionDefinitionSO : ScriptableObject
    {
        [ReadOnly] public int id;
        
        [SerializeField] private Color color = Color.white;
        [SerializeField] private FactionDefinitionSO[] allies;
        [SerializeField] private FactionDefinitionSO[] enemies;
        
        public int Id => id;
        public Color Color => color;
        public FactionDefinitionSO[] Allies => allies;
        public FactionDefinitionSO[] Enemies => enemies;
    }
}