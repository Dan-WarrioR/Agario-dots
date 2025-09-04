using System;
using Core.SceneManagement;
using Data;
using Features.UI.ScreenManagement;
using Features.UI.ScreenManagement.Screens;
using HSM;
using ProjectTools.DependencyManagement;
using Unity.Entities;
using UnityEngine;
using SceneReference = Core.SceneManagement.SceneReference;

namespace Core.States
{
    public interface ILoadableState
    {
        public SceneLoadingState.Data Data { get; }
    }
    
    public class SceneLoadingState : BaseSubState<SceneLoadingState, AppHsm>
    {
        public class Data
        {
            public readonly string sceneName;
            public readonly ISubState<AppHsm> nextState;
            public Action<SceneReference> onSceneLoaded;

            public Data(string sceneName, ISubState<AppHsm> nextState)
            {
                this.sceneName = sceneName;
                this.nextState = nextState;
            }
        }
        
        private readonly Data _data;
        
        private ScreenManager _screenManager;

        private SceneLoadingState(Data data)
        {
            _data = data;
        }
        
        public static void TransitionTo(AppHsm app, ILoadableState targetState)
        {
            app.SetSubState(new SceneLoadingState(targetState.Data));
        }
        
        public override void OnEnter(SystemBase system)
        {
            _screenManager = Service.Get<ScreenManager>();
            _screenManager.Open<LoadingScreen>();
            
            SceneLoader.UnloadAllScenes(() =>
            {
                SceneLoader.LoadScene(_data.sceneName, OnSceneLoaded);;
            });
        }

        public override void OnExit(SystemBase system)
        {
            _screenManager.CloseCurrent();
        }
        
        private void OnSceneLoaded(SceneReference reference)
        {
            SceneLoader.ForceActiveScene = reference.Scene;

            _data.onSceneLoaded?.Invoke(reference);

            if (_data.nextState != null)
            {
                Parent.SetSubState(_data.nextState);
            }
        }
    }
}