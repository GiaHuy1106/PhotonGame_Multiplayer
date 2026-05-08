using UnityEngine;

public class Attack02 : IState
{
    PlayerContext ctx;
    Animator anim;
    public Attack02(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter Attack 2");
        if(anim == null)
        {
            anim = ctx.anim.GetAnimator();
        }
        ctx.anim.PlayClip(PlayerAnimatorController.ATTACK02_HASH, 1);
    }

    public void Execute(float tick)
    {
        var stateInfo = anim.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName(nameof(Attack02)) && stateInfo.normalizedTime >= .99f)
        {
            ctx.ChangeCombatState(nameof(NoneState));
        }
    }

    public void Exit()
    {
    }
}
