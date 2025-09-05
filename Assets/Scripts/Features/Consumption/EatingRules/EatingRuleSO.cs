using UnityEngine;

namespace Features.Consumption.EatingRules
{
    public enum EatingRuleType
    {
        Faction,
    }
    
    [CreateAssetMenu(menuName = "Data/EatingRule")]
    public class EatingRuleSO : ScriptableObject
    {
        [SerializeField]
        private EatingRuleType ruleType;

        public TryEatRuleDelegate GetRule()
        {
            return ruleType switch
            {
                EatingRuleType.Faction => FactionEatingRule.TryEat,
            };
        }
    }
}