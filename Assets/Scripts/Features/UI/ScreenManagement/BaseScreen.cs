using System.Collections.Generic;
using DG.Tweening;
using Unity.Entities;
using UnityEngine;

namespace Features.UI
{
    public abstract class BaseScreen : MonoBehaviour
    {
        //////////////////////////////////////////////////
        
        #region Data
        
        [SerializeField] private List<BaseScreen> children = new();
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.25f;
        
        public BaseScreen Parent { get; private set; }
        public IReadOnlyList<BaseScreen> Children => children;
        
        #endregion
        
        //////////////////////////////////////////////////
        
        #region Overrides
        
        protected virtual void OnCreate(SystemBase system) { }
        protected virtual void OnUpdate(SystemBase system) { }
        protected virtual void OnExit(SystemBase system) { }
        
        #endregion
        
        //////////////////////////////////////////////////
        
        #region Interface
        
        public T AddChild<T>(T prefab, SystemBase system, TweenCallback callback = null) where T : BaseScreen
        {
            var screen = Instantiate(prefab, transform);
            screen.gameObject.SetActive(false);
            screen.AttachToParent(this);
            screen.OnCreate(system);
            screen.gameObject.SetActive(true);
            screen.PlayFadeIn(callback);
            return screen;
        }
        
        public void RemoveSubtree(SystemBase system, TweenCallback onComplete = null)
        {
            if (children.Count == 0)
            {
                RemoveSelf(system, onComplete);
                return;
            }

            int remaining = children.Count;
            TweenCallback childCallback = null;
            childCallback = () =>
            {
                remaining--;
                if (remaining == 0)
                {
                    RemoveSelf(system, onComplete);
                }
            };

            for (int i = children.Count - 1; i >= 0; i--)
            {
                var child = children[i];
                child.RemoveSubtree(system, childCallback);
            }
        }
        
        public void PlayFadeIn(TweenCallback callback = null)
        {
            DOTween.Kill(canvasGroup);
            gameObject.SetActive(true);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                callback?.Invoke();
            });
        }

        public void PlayFadeOut(TweenCallback callback = null)
        {
            DOTween.Kill(canvasGroup);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            });
        }
        
        #endregion
        
        
        //////////////////////////////////////////////////
        
        #region Internal
        
        internal void AttachToParent(BaseScreen parent)
        {
            Parent = parent;
            parent.children.Add(this);
        }
        
        internal void DetachFromParent()
        {
            if (Parent != null)
            {
                Parent.children.Remove(this);
                Parent = null;
            }
        }
        
        internal bool ContainsChild(BaseScreen child)
        {
            return children.Contains(child);
        }

        internal void InternalOnCreate(SystemBase system)
        {
            OnCreate(system);
        }
        
        internal void InternalOnExit(SystemBase system)
        {
            OnExit(system);
        }
        
        internal void InternalUpdate(SystemBase system)
        {
            OnUpdate(system);
            
            int count = children.Count;
            for (int i = 0; i < count; i++)
            {
                children[i].InternalUpdate(system);
            }
        }
        
        #endregion
        
        //////////////////////////////////////////////////
        
        #region Private
        
        private void RemoveSelf(SystemBase system, TweenCallback callback = null)
        {
            OnExit(system);
            DetachFromParent();
            
            PlayFadeOut(() =>
            {
                Destroy(gameObject);
                callback?.Invoke();
            });
        }

        #endregion
        
        //////////////////////////////////////////////////
    }
}