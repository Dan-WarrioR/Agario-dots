using System;
using Core.SceneManagement;
using HSM;
using Unity.Entities;
using SceneReference = Core.SceneManagement.SceneReference;

namespace Core.States
{
    public class SceneLoadingState : BaseSubState<SceneLoadingState, AppHsm>
    {
        private readonly string _sceneName;
        private readonly ISubState<AppHsm> _nextState;
        private readonly Action<SceneReference> _onSceneLoaded;
        
        public  SceneLoadingState(string sceneName, ISubState<AppHsm> nextState, Action<SceneReference> onSceneLoaded = null)
        {
            _sceneName = sceneName;
            _nextState = nextState;
            _onSceneLoaded = onSceneLoaded;
        }
        
        public override void OnEnter(SystemBase system)
        {
            SceneLoader.UnloadAllScenes(() =>
            {
                SceneLoader.LoadScene(_sceneName, OnSceneLoaded);
            });
        }

        public override void OnExit(SystemBase system)
        {
            
        }
        
        private void OnSceneLoaded(SceneReference reference)
        {
            SceneLoader.ForceActiveScene = reference.Scene;
            _onSceneLoaded?.Invoke(reference);

            if (_nextState != null)
            {
                Parent.SetSubState(_nextState);
            }
        }
    }
}