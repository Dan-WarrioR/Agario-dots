using System;
using ProjectTools.DependencyManagement;
using Unity.Entities;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GlobalConfigs", menuName = "Data/GlobalConfigs")]
    [Serializable]
    public class GlobalConfigs : BaseSOAuthoring
    {
        [SerializeField] private GameplayConfig gameplayConfig;
        [SerializeField] private AIBehaviourConfig aiBehaviourConfig;
        
        [SerializeField] private UIConfig uiConfig;
        
        public override void Bake(World world)
        {
            world.EntityManager.CreateSingleton(gameplayConfig);
            world.EntityManager.CreateSingleton(aiBehaviourConfig);
            
            Service.Register(this);
            Service.Register(uiConfig);
        }
    }
}