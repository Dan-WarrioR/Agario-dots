using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Features.UI.ScreenManagement
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class ScreenSystem : SystemBase
    {
        private readonly List<BaseScreen> _roots = new();
        private readonly Dictionary<Type, BaseScreen> _screenPrefabs = new();
        private UIContainer _screenContainer;
        
        protected override void OnUpdate()
        {
            for (int i = _roots.Count - 1; i >= 0; i--)
            {
                _roots[i].InternalUpdate(this);
            }
        }
        
        //////////////////////////////////////////////////

        #region Interface

        public void SetContainer(UIContainer container)
        {
            _screenContainer = container;
        }
        
        public void SetConfig(ScreenConfigSO screenConfig)
        {
            foreach (var prefab in screenConfig.ScreenPrefabs)
            {
                if (!_screenPrefabs.TryAdd(prefab.GetType(), prefab))
                {
                    Debug.LogWarning($"{prefab.name} is already registered!");
                }
            }
        }
        
        public T OpenScreen<T>(TweenCallback callback = null) where T : BaseScreen
        {
            if (!_screenPrefabs.TryGetValue(typeof(T), out var prefab))
            {
                Debug.LogError($"Prefab for {typeof(T).Name} not found!");
                return null;
            }

            var screen = Object.Instantiate(prefab, _screenContainer.Container) as T;
            screen.gameObject.SetActive(false);
            screen.InternalOnCreate(this);
            screen.gameObject.SetActive(true);
            screen.PlayFadeIn(callback);

            _roots.Add(screen);
            return screen;
        }
        
        public void CloseScreen<T>(TweenCallback callback = null) where T : BaseScreen
        {
            var root = _roots.OfType<T>().FirstOrDefault();
            if (root == null)
            {
                Debug.LogWarning($"[CloseScreen] Root {typeof(T).Name} not found!");
                callback?.Invoke();
                return;
            }

            _roots.Remove(root);
            root.RemoveSubtree(this, callback);
        }
        
        public TChild OpenSubScreen<TChild, TParent>(TweenCallback callback = null)
            where TChild : BaseScreen
            where TParent : BaseScreen
        {
            if (!_screenPrefabs.TryGetValue(typeof(TChild), out var prefab))
            {
                Debug.LogError($"Prefab for {typeof(TChild).Name} not found!");
                return null;
            }

            var parent = FindInTree<TParent>();
            if (parent == null)
            {
                Debug.LogError($"Parent {typeof(TParent).Name} not found!");
                return null;
            }

            return parent.AddChild(prefab, this, callback) as TChild;
        }
        
        public void CloseSubScreen<TChild, TParent>(TweenCallback callback = null)
            where TChild : BaseScreen
            where TParent : BaseScreen
        {
            var parent = FindInTree<TParent>();
            if (parent == null)
            {
                Debug.LogWarning($"Parent {typeof(TParent).Name} not found!");
                callback?.Invoke();
                return;
            }
            
            var child = parent.Children.OfType<TChild>().FirstOrDefault();
            if (child == null)
            {
                Debug.LogWarning($"Child {typeof(TChild).Name} not found under {typeof(TParent).Name}!");
                callback?.Invoke();
                return;
            }

            child.RemoveSubtree(this, callback);
        }

        #endregion
        
        //////////////////////////////////////////////////
        
        private T FindInTree<T>() where T : BaseScreen
        {
            foreach (var root in _roots)
            {
                var found = FindRecursive<T>(root);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private T FindRecursive<T>(BaseScreen node) where T : BaseScreen
        {
            if (node is T target)
            {
                return target;
            }

            foreach (var child in node.Children)
            {
                var found = FindRecursive<T>(child);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}