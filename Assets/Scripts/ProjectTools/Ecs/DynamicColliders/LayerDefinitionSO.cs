using Unity.Collections;
using UnityEngine;

namespace ProjectTools.Ecs.DynamicColliders
{
    [CreateAssetMenu(menuName = "Data/LayerDefinitionSO")]
    public class LayerDefinitionSO : ScriptableObject
    {
        [ReadOnly] public int id;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private LayerDefinitionSO[] interactsWith;

        public int Id => id;
        public Color Color => color;
        public LayerDefinitionSO[] InteractsWith => interactsWith;
    }
}