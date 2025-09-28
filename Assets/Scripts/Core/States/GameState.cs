using HSM;
using Unity.Entities;

namespace Core.States
{
    public class GameState : BaseSubState<GameState, AppHsm>
    {
        public const string GameSceneName = "SampleScene";
        
        public override void OnEnter(SystemBase system)
        {
            SetSubState(new PlayingGameState());
        }
    }
}