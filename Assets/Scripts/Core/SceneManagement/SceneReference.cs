using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.SceneManagement
{
    public class SceneReference : MonoBehaviour
    {
        [SerializeField]
        private bool isGlobalScene = false;
        
        public Scene Scene => gameObject.scene;
        public bool IsGlobalScene => isGlobalScene;

        private void Awake()
        {
            SceneLoader.RegisterReference(this);
        }

        private void OnDestroy()
        {
            SceneLoader.UnregisterReference(this);
        }
    }
}