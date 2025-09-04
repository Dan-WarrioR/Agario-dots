using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Features.UI.ScreenManagement.Screens;
using ProjectTools.DependencyManagement;
using UnityEngine;

namespace Features.UI.ScreenManagement
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private UIConfig uiConfig;
        [SerializeField] private Transform screenContainer;
        [SerializeField] private float fadeDuration = 0.25f;

        private BaseScreen CurrentScreen => _screens.Count > 0 ? _screens.Peek() : null;

        private readonly Stack<BaseScreen> _screens = new();
        private Dictionary<Type, BaseScreen> _prefabs = new();
        
        
        
        private void Awake()
        {
            SetupPrefabs();
            Service.Register(this);
        }
        
        private void OnDestroy()
        {
            Service.Unregister(this);
        }

        private void SetupPrefabs()
        {
            foreach (var screen in uiConfig.Screens)
            {
                _prefabs.TryAdd(screen.GetType(), screen);
            }
        }
        

        public void OpenNow<T>(Action<T> callback = null) where T : BaseScreen
        {
            CloseAll();
            Open(callback);
        }
        
        public void Open<T>(Action<T> callback = null) where T : BaseScreen
        {
            OpenInternal(typeof(T), callback);
        }
        
        public void CloseCurrent()
        {
            if (_screens.Count == 0)
            {
                Debug.LogError("Trying to close empty screens collection!");
                return;
            }
            
            var currentScreen = _screens.Pop();
            currentScreen.Close(fadeDuration, () =>
            {
                Destroy(currentScreen.gameObject);
                CurrentScreen?.Open(fadeDuration);
            });
        }
        
        public void CloseAll(Action callback = null)
        {
            if (_screens.Count == 0)
            {
                Debug.LogWarning("Screens collection is already empty!");
                return;
            }
            
            var currentScreen = _screens.Pop();

            currentScreen.Close(fadeDuration, () =>
            {
                Destroy(currentScreen.gameObject);
            });

            foreach (var screen in _screens)
            {
                Destroy(screen.gameObject);
            }

            _screens.Clear();
        }
        
        
        private void OpenInternal<T>(Type screenType, Action<T> callback = null) where T : BaseScreen
        {
            if (!_prefabs.TryGetValue(screenType, out var prefab))
            {
                Debug.LogError($"No screen prefab found for type {screenType.Name}");
                return;
            }

            if (CurrentScreen != null)
            {
                CurrentScreen.Close(fadeDuration, () =>
                {
                    var screen = InstantiateScreen(prefab);
                    callback?.Invoke(screen as T);
                });
            }
            else
            {
                var screen = InstantiateScreen(prefab);
                callback?.Invoke(screen as T);
            }
        }
        
        private BaseScreen InstantiateScreen(BaseScreen screen)
        {
            var instance = Instantiate(screen, screenContainer);
            instance.Construct(this);
            _screens.Push(instance);
            instance.Open(fadeDuration);
            return instance;
        }
    }
}