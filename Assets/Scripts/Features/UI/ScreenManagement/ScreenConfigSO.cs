using System.Collections.Generic;
using UnityEngine;

namespace Features.UI.ScreenManagement
{
    [CreateAssetMenu(menuName = "Data/ScreenConfig")]
    public class ScreenConfigSO : ScriptableObject
    {
        [SerializeField] private List<BaseScreen> screenPrefabs = new();

        public List<BaseScreen> ScreenPrefabs => screenPrefabs;
    }
}