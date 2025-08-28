using HSM;
using Unity.Entities;
using UnityEngine;

namespace Core.States
{
    public class BootState : BaseSubState<BootState, AppHsm>
    {
        public override void OnEnter(SystemBase system)
        {
            Debug.Log("Enter BootState");
            SceneLoadingState.TransitionTo(Parent, new MainMenuState());
        }
    }
}