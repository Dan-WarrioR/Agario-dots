using Core.SceneManagement;
using Core.States;
using Data;
using HSM;
using Unity.Entities;
using UnityEngine;

namespace Core
{
    public class Boot : MonoBehaviour
    {
        private enum InitState
        {   
            Boot, 
            MainMenu, 
            Game,
        }
        
        private static bool IsPreloaded = false;
        
        [SerializeField] private GlobalConfigs globalConfigs;

        [SerializeField] private string coreSceneName = "Core";
        [SerializeField] private InitState initState;
        
        private void Awake()
        {
            if (IsPreloaded)
            {
                return;
            }
            
            SceneLoader.Initialize();
            
            IsPreloaded = true;
            
            Application.targetFrameRate = 60;
            globalConfigs.Bake(World.DefaultGameObjectInjectionWorld);
            
            SceneLoader.LoadScene(coreSceneName, _ =>
            {
                HsmTools.InitHsm(GetInitState());
            });
        }
        
        private ISubState<AppHsm> GetInitState()
        {
            return initState switch
            {
                InitState.Boot => new BootState(),
                InitState.MainMenu => new MainMenuState(),
                InitState.Game => new GameState(),
                _ => new BootState(),
            };
        }
    }
}