using UnityEngine;

public class JumpStart : IState
{
    PlayerContext ctx;
    public JumpStart(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter JumpStart");
        ctx.anim.PlayClip(PlayerAnimatorController.JUMPSTART_HASH);
    }

    public void Execute(float tick)
    {
        if(ctx.controller.Velocity.y < 0 && !ctx.controller.Grounded)
        {
            ctx.ChangeMovementState(nameof(Fall));
        }
    }

    public void Exit()
    {
    }
}
