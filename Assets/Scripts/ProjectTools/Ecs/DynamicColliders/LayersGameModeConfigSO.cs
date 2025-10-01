using UnityEngine;

namespace ProjectTools.Ecs.DynamicColliders
{
    [CreateAssetMenu(menuName = "Layers/LayersGameMode")]
    public class LayersGameModeConfigSO : ScriptableObject
    {
        [Tooltip("Список фракцій для цього режиму")]
        public LayerDefinitionSO[] layers;
        
    }
}
