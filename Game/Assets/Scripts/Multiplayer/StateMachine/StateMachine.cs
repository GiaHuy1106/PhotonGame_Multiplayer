using UnityEngine;

public class StateMachine
{

    IState current;


    public void ChangeState(IState newState)
    {
        Debug.Log("ChangeState");
        if (newState != null && current != newState)
        {
            current?.Exit();
            current = newState;
            current.Enter();
        }

    }
    public void Update(float tick)
    {
        if (current != null)
        {
            current.Execute(tick);
        }
    }
}
