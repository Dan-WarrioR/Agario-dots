using HSM;
using Unity.Entities;

namespace Core.States
{
    public class BootState : BaseSubState<BootState, AppHsm>
    {
        public override void OnEnter(SystemBase system)
        {
            Parent.SetSubState(new SceneLoadingState(MainMenuState.MainMenuSceneName, new MainMenuState()));
        }
    }
}