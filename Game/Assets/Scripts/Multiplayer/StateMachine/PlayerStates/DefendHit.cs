using UnityEngine;

public class DefendHit : IState
{
    PlayerContext ctx;
    public DefendHit(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter defend hit ");
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
