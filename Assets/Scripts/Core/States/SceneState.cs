using Core.SceneManagement;
using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using Unity.Entities;

namespace Core.States
{
    public abstract class SceneState<TSelf, TParent> : BaseSubState<TSelf, TParent>
        where TSelf : SceneState<TSelf, TParent>
        where TParent : BaseState
    {
        protected abstract string SceneName { get; }
        
        public sealed override void OnEnter(SystemBase system)
        {
            ScreenAPI.OpenScreen<LoadingScreen>(system.World);
            SceneLoader.UnloadAllScenes(() =>
            {
                SceneLoader.LoadScene(SceneName, reference =>
                {
                    SceneLoader.ForceActiveScene = reference.Scene;

                    ScreenAPI.CloseScreen<LoadingScreen>(system.World);

                    OnSceneEnter(system);
                });
            });
        }

        protected abstract void OnSceneEnter(SystemBase system);
    }
}