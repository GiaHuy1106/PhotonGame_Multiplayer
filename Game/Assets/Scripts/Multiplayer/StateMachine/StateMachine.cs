public class StateMachine
{

    IState current;


    public void ChangeState(IState newState)
    {
        if (newState != null && current != newState)
        {
            current?.Exit();
            current = newState;
            current.Enter();
        }

    }
    public void Update()
    {
        if (current != null)
        {
            current.Execute();
        }
    }
}
