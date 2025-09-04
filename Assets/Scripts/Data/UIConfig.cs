using System.Collections.Generic;
using Features.UI.ScreenManagement.Screens;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Data/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [SerializeField] private List<BaseScreen> screens;
        
        public IReadOnlyList<BaseScreen> Screens => screens;
    }
}