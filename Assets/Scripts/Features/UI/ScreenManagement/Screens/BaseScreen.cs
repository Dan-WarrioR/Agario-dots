using System;
using DG.Tweening;
using UnityEngine;

namespace Features.UI.ScreenManagement.Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseScreen : MonoBehaviour
    {
        public enum ScreenMode
        {
            Single,
            Additive,
        }
        
        [SerializeField] private ScreenMode screenMode = ScreenMode.Single;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public ScreenMode Mode => screenMode;
        protected ScreenManager Manager { get; private set; }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
        }
#endif
        
        internal void Construct(ScreenManager manager)
        {
            Manager = manager;
        }
        
        public virtual void Open(float duration, Action callback = null)
        {
            gameObject.SetActive(true);

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(1f, duration).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                callback?.Invoke();
            });
        }
        
        public virtual void Close(float duration, Action callback = null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0f, duration).OnComplete(() =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            });
        }
    }
}