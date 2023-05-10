public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void Update() => CurrentState?.UpdateState();

    public void ChangeState(IState newState)
    {
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }

    public interface IState
    {
        void OnEnter();

        void UpdateState();

        void OnExit();

    }
}