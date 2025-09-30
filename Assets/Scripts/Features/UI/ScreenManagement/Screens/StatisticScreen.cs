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
        
        private List<(bool active, Color color, string text)> _lastStates;
        private float _timer = 0f;

        protected override void OnCreate(SystemBase system)
        {
            _lastStates = new(texts.Count);
            InitializeLastStates();
        }

        protected override void OnUpdate(SystemBase system)
        {
            TryUpdateLayout(system);
        }

        private void TryUpdateLayout(SystemBase system)
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
            if (!system.TryGetSingleton(out LayerDatabaseComponent layerDatabase))
            {
                return;
            }
            
            var singleton = system.GetSingletonEntity<StatisticsSingleton>();
            var buffer = system.EntityManager.GetBuffer<CountPerLayer>(singleton, true);

            ValidateLastStateCount();
            
            for (int i = 0; i < texts.Count; i++)
            {
                if (i < buffer.Length)
                {
                    UpdateActiveText(i, buffer[i], layerDatabase);
                }
                else
                {
                    DeactivateText(i);
                }
            }
        }
        
        private void DeactivateText(int index)
        {
            var state = _lastStates[index];
            if (!state.active)
            {
                return;
            }

            texts[index].gameObject.SetActive(false);
            _lastStates[index] = (false, state.color, state.text);
        }
        
        private void UpdateActiveText(int index, CountPerLayer data, LayerDatabaseComponent layerDatabase)
        {
            var state = _lastStates[index];
            var layerId = data.layerId;
            var color = LayerUtility.GetColor(ref layerDatabase.blob.Value, layerId);
            var newText = $"Layer {layerId}: {data.count}";

            if (state.active && state.color == color && state.text == newText)
            {
                return;
            }
            
            var text = texts[index];
            text.gameObject.SetActive(true);
            text.color = color;
            text.text = newText;
            _lastStates[index] = (true, color, newText);
        }
        
        private void ValidateLastStateCount()
        {
            if (_lastStates.Count >= texts.Count)
            {
                return;
            }

            int toAdd = texts.Count - _lastStates.Count;
            for (int j = 0; j < toAdd; j++)
            {
                _lastStates.Add((false, default, string.Empty));
            }
        }
        
        private void InitializeLastStates()
        {
            _lastStates.Clear();
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].gameObject.SetActive(false);
                _lastStates.Add((false, default, string.Empty));
            }
        }
    }
}