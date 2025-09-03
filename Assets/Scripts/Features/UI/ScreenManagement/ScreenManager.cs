using System;
using System.Collections.Generic;
using Features.UI.ScreenManagement.Screens;
using ProjectTools.DependencyManagement;
using UnityEngine;

namespace Features.UI.ScreenManagement
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private Transform screenContainer;
        [SerializeField] private float fadeDuration = 0.25f;

        private readonly List<BaseScreen> _screens = new();


        
        private void Awake()
        {
            Service.Register(this);
        }
        
        private void OnDestroy()
        {
            Service.Unregister(this);
        }
        
        
        
        public T Open<T>(T prefab, Action callback = null) where T : BaseScreen
        {
            if (_screens.Contains(prefab))
            {
                Debug.LogWarning($"Screen {prefab.name} already open!");
                return null;
            }
            
            T instance = null;
            
            if (prefab.Mode == BaseScreen.ScreenMode.Single)
            {
                CloseAll(() =>
                {
                    instance = InstantiateScreen(prefab, callback);
                });
            }
            else
            {
                instance = InstantiateScreen(prefab, callback);
            }

            return instance;
        }
        
        public void Close(BaseScreen screen, Action callback = null)
        {
            screen.Close(fadeDuration, () =>
            {
                _screens.Remove(screen);
                Destroy(screen.gameObject);
                callback?.Invoke();
            });
        }
        
        public void CloseAll(Action callback = null)
        {
            int baseCount = _screens.Count;
            if (baseCount == 0)
            {
                callback?.Invoke();
                return;
            }

            foreach (var screen in _screens)
            {
                Close(screen, () =>
                {
                    baseCount--;
                    if (baseCount == 0)
                    {
                        callback?.Invoke();
                    }
                });
            }
        }
        
        
        
        private T InstantiateScreen<T>(T prefab, Action callback) where T : BaseScreen
        {
            var instance = Instantiate(prefab, screenContainer);
            
            instance.Construct(this);
            instance.transform.SetAsLastSibling();
            _screens.Add(instance);
            instance.Open(fadeDuration, callback);

            return instance;
        }
    }
}