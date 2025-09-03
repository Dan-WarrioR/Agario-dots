using Features.UI.ScreenManagement.Screens;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Data/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        public PauseScreen pauseScreen;
        public LoadingScreen loadingScreen;
        public MainMenuScreen mainMenuScreen;
    }
}