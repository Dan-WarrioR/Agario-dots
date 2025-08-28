using HSM;
using Unity.Entities;

namespace Core.States
{
    public class GameState : BaseSubState<GameState, AppHsm>, ILoadableState
    {
        private const string GameSceneName = "SampleScene";

        public SceneLoadingState.Data Data => new(GameSceneName, this);

        
        public override void OnEnter(SystemBase system)
        {
            SetSubState(new PlayingGameState());
        }
    }
}