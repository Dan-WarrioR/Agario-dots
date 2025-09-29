using HSM;
using Unity.Entities;

namespace Core.States
{
    public class GameState : SceneState<GameState, AppHsm>
    {
        protected override string SceneName => "SampleScene";

        protected override void OnSceneEnter(SystemBase system)
        {
            SetSubState(new PlayingGameState());
        }
    }
}