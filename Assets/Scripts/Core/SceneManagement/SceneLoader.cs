using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.SceneManagement
{
    public static class SceneLoader
    {
        //////////////////////////////////////////////////

        #region Data

        private const string HotReferenceName = "HotReference";
        private const LoadSceneMode SceneLoadMode = LoadSceneMode.Additive;

        private static readonly HashSet<SceneReference> SceneReferences = new();

        private static Scene _forceActiveScene;
        private static Scene _previousActive;
        
        public static Scene ForceActiveScene
        {
            get => _forceActiveScene;
            set
            {
                _previousActive = _forceActiveScene;
                _forceActiveScene = value;
                if (_forceActiveScene.IsValid() && _forceActiveScene.isLoaded)
                {
                    SceneManager.SetActiveScene(_forceActiveScene);
                }
            }
        }
        
        #endregion
        
        //////////////////////////////////////////////////

        #region Interface

        public static void Initialize()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            RegisterReferences();
        }
        
        public static bool IsSceneLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).IsValid();
        }
        
        public static void LoadScene(string sceneName, Action<SceneReference> callback = null, bool allowSceneActivation = true)
        {
            if (TryGetSceneReference(sceneName, out _))
            {
                Debug.LogWarning($"Scene with name {sceneName} already exists!");
                return;
            }
            
            var operation = SceneManager.LoadSceneAsync(sceneName, SceneLoadMode);
            operation.allowSceneActivation = allowSceneActivation;
            operation.completed += OnLoad;

            void OnLoad(AsyncOperation asyncOperation)
            {
                asyncOperation.completed -= OnLoad;
                var scene = SceneManager.GetSceneByName(sceneName);
                var reference = SetupSceneReference(scene);
                callback?.Invoke(reference);
            }
        }

        public static void UnloadScene(string sceneName, Action callback = null)
        {
            if (!TryGetSceneReference(sceneName, out _))
            {
                Debug.LogWarning($"Scene with name {sceneName} doesn't exist!");
                return;
            }
            
            var operation = SceneManager.UnloadSceneAsync(sceneName);
            operation.completed += OnUnload;

            void OnUnload(AsyncOperation asyncOperation)
            {
                asyncOperation.completed -= OnUnload;
                callback?.Invoke();
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        public static void UnloadAllScenes(Action callback = null)
        {
            int baseCount = SceneReferences.Count;
            int unloadedCount = 0;
            bool isUnloading = false;
            
            foreach (var reference in SceneReferences)
            {
                if (reference.IsGlobalScene)
                {
                    baseCount--;
                    continue;
                }

                isUnloading = true;
                
                var operation = SceneManager.UnloadSceneAsync(reference.Scene);
                if (operation != null)
                {
                    operation.completed += OnUnload;
                }
                else
                {
                    OnUnload(null);
                }
            }

            if (!isUnloading)
            {
                callback?.Invoke();
            }

            void OnUnload(AsyncOperation asyncOperation)
            {
                if (asyncOperation != null)
                {
                    asyncOperation.completed -= OnUnload;
                }

                if (baseCount <= ++unloadedCount)
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    callback?.Invoke();
                }
            }
        }
        
        #endregion
        
        //////////////////////////////////////////////////
        
        #region Scene Reference Registration
        
        internal static void RegisterReference(SceneReference reference)
        {
            if (!SceneReferences.Add(reference))
            {
                return;
            }
        }
        
        internal static void UnregisterReference(SceneReference reference)
        {
            if (SceneReferences.Contains(reference))
            {
                SceneReferences.Remove(reference);
            }
        }
        
        #endregion
        
        //////////////////////////////////////////////////

        #region Private Implementation

        private static void OnActiveSceneChanged(Scene prev, Scene next)
        {
            if (!ForceActiveScene.isLoaded || next == ForceActiveScene)
            {
                return; 
            }
            SceneManager.SetActiveScene(ForceActiveScene);
        }
        
        private static bool TryGetSceneReference(string sceneName, out SceneReference sceneReference)
        {
            foreach (var reference in SceneReferences)
            {
                if (reference.Scene.name != sceneName)
                {
                    continue;
                }

                sceneReference = reference;
                return true;
            }

            sceneReference = null;
            return false;
        }

        private static SceneReference SetupSceneReference(Scene scene)
        {
            if (TryGetSceneReference(scene.name, out var sceneReference))
            {
                return sceneReference;
            }
            
            var gameObject = new GameObject(HotReferenceName);
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            return gameObject.AddComponent<SceneReference>();
        }

        private static void RegisterReferences()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    SetupSceneReference(scene);
                }
            }
        }
        
        #endregion
        
        //////////////////////////////////////////////////
    }
}