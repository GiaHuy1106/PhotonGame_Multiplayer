using UnityEngine;

public class Die : IState
{
    PlayerContext ctx;
    public Die(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter Die");
    }

    public void Execute(float tick)
    {

    }

    public void Exit()
    {
    }
}
