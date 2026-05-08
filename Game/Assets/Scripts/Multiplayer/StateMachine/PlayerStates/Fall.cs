using UnityEngine;

public class Fall : IState
{
    PlayerContext ctx;
    public Fall(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter Fall");
        ctx.anim.PlayClip(PlayerAnimatorController.FALL_HASH);
    }

    public void Execute(float tick)
    {
        if (ctx.controller.Grounded)
        {
            ctx.ChangeMovementState(nameof(JumpEnd));
        }
    }

    public void Exit()
    {
    }
}
