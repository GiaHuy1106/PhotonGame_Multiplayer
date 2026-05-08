using UnityEngine;

public class JumpEnd : IState
{
    PlayerContext ctx;
    Animator anim;
    public JumpEnd(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter JumpEnd");
        if(anim == null)
        {
            anim = ctx.anim.GetAnimator();
        }
        ctx.anim.PlayClip(PlayerAnimatorController.JUMPEND_HASH);
    }

    public void Execute(float tick)
    {
        
        Vector3 velocity = ctx.controller.Velocity;
        velocity.x /= 2;
        velocity.z /= 2;
        ctx.controller.Velocity = velocity;
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName(nameof(JumpEnd)) && stateInfo.normalizedTime >= .99f)
        {
            ctx.ChangeMovementState(nameof(Idle));
        }
       
    }

    public void Exit()
    {
    }

  
}
