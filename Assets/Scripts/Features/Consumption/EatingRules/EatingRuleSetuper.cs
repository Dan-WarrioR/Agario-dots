using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Features.Consumption.EatingRules
{
    public class EatingRuleSetuper : MonoBehaviour
    {
        [SerializeField]
        private List<EatingRuleSO> rules;

        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var system = world.GetOrCreateSystemManaged<EatingSystem>();

            foreach (var rule in rules)
            {
                system.AddRule(rule.GetRule());
            }
        }
    }
}