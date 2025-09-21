#if UNITY_EDITOR
using System.Linq;
using ProjectTools.Ecs.DynamicColliders;
using UnityEditor;

namespace Features.Faction
{
    [InitializeOnLoad]
    public static class EditorFactionValidator
    {
        static EditorFactionValidator()
        {
            EditorApplication.projectChanged += ReassignFactionIds;
            ReassignFactionIds();
        }

        private static void ReassignFactionIds()
        {
            string[] guids = AssetDatabase.FindAssets($"t={nameof(LayerDefinitionSO)}");
            var factions = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<LayerDefinitionSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(f => f != null)
                .OrderBy(f => f.id)
                .ToList();

            int expectedId = 0;
            foreach (var faction in factions)
            {
                if (faction.id != expectedId)
                {
                    faction.id = expectedId;
                    EditorUtility.SetDirty(faction);
                }
                expectedId++;
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif