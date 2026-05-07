using UnityEngine;

public interface IState 
{
    void Enter();
    void Execute(float tick);
    void Exit();
}
