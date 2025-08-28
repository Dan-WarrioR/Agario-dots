using HSM;
using Unity.Entities;

namespace Core.States
{
    public class MainMenuState : BaseSubState<MainMenuState, AppHsm>, ILoadableState
    {
        private const string MainMenuSceneName = "MainMenu";

        public SceneLoadingState.Data Data => new(MainMenuSceneName, this);

        public override void OnUpdate(SystemBase system)
        {
 
        }
    }
}