// State pattern: common interface for every enemy AI state.
// Each concrete state encapsulates its own per-frame behaviour (Execute)
// and its own transition rules, replacing the old
// "enum + StartCoroutine(enemyState.ToString())" string-driven FSM.
public interface IEnemyState
{
    EnemyState StateType { get; }

    void Enter();    // called once when the state becomes active
    void Execute();  // called every frame while the state is active
    void Exit();     // called once when the state is left
}
