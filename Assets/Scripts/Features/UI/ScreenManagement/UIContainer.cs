using Unity.Entities;
using UnityEngine;

namespace Features.UI.ScreenManagement
{
    [RequireComponent(typeof(Canvas))]
    public class UIContainer : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform container;
        [SerializeField] private ScreenConfigSO screenConfig;

        public RectTransform Container => container;

        private void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var system = world.GetExistingSystemManaged<ScreenSystem>();
            
            system.SetContainer(this);
            system.SetConfig(screenConfig);
        }
    }
}