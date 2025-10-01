using Unity.Collections;
using UnityEngine;

namespace ProjectTools.Ecs.DynamicColliders
{
    [CreateAssetMenu(menuName = "Data/LayerDefinitionSO")]
    public class LayerDefinitionSO : ScriptableObject
    {
        [ReadOnly] public int id;
        [SerializeField] private Color color = Color.black;
        [SerializeField] private Sprite factionSprite;
        [SerializeField] private LayerDefinitionSO[] interactsWith;
        [SerializeField] private ScriptableObject behavior;

        public int Id => id;
        public Color Color => color;
        public Sprite FactionSprite => factionSprite;
        public LayerDefinitionSO[] InteractsWith => interactsWith;
        public ScriptableObject Behavior => behavior;
    }
}