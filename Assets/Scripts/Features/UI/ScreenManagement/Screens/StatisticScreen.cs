using System.Collections.Generic;
using ProjectTools.Ecs.DynamicColliders;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Features.UI.ScreenManagement.Screens
{
    public class StatisticScreen : BaseScreen
    {
        [SerializeField] private List<TMP_Text> texts;
        [SerializeField] private float updateInterval = 0.25f;
        
        private float _timer = 0f;
        
        protected override void OnUpdate(SystemBase system)
        {
            _timer += Time.deltaTime;
            if (_timer < updateInterval)
            {
                return;
            }

            _timer = 0f;

            UpdateLayout(system);
        }

        private void UpdateLayout(SystemBase system)
        {
            var singleton = system.GetSingletonEntity<StatisticsSingleton>();
            var buffer = system.EntityManager.GetBuffer<CountPerLayer>(singleton, true);
            ref var blobAsset = ref system.GetSingleton<LayerDatabaseComponent>().blob.Value;
            
            for (int i = 0; i < texts.Count; i++)
            {
                var text = texts[i];
                if (i >= buffer.Length)
                {
                    text.gameObject.SetActive(false);
                    continue;
                }
                
                text.gameObject.SetActive(true);
                text.color = LayerUtility.GetColor(ref blobAsset, i);
                text.text = $"Layer {buffer[i].layerId}: {buffer[i].count}";
            }
        }
    }
}