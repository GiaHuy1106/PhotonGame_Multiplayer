using UnityEngine;

public class NoneState : IState
{
    PlayerContext ctx;
    public NoneState(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter NoneState");
        ctx.anim.PlayClip(PlayerAnimatorController.NONE_HASH, 1);
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
