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
        ctx.anim.PlayClip(PlayerAnimatorController.DIE_HASH);
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
