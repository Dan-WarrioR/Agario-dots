using HSM;
using Unity.Entities;
using UnityEngine;

namespace Core.States
{
    public class GameState : BaseSubState<GameState, AppHsm>
    {
        public override void OnEnter(SystemBase system)
        {
            Debug.Log("Enter GameState");
            SetSubState(new PlayingGameState());
        }
        
        public override void OnExit(SystemBase system)
        {
            Debug.Log("Exit GameState");
        }
    }
}