using UnityEngine;

public class Attack01 : IState
{
    PlayerContext ctx;
    Animator animator;
    public Attack01(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        if (animator == null) animator = ctx.anim.GetAnimator();
    }

    public void Execute(float tick)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        if(stateInfo.IsName(nameof(Attack01)) && stateInfo.normalizedTime >= .85f){
            if (ctx.inputData.isAttack)
            {
                ctx.ChangeCombatState(nameof(Attack02));
            }
            else if(stateInfo.normalizedTime >= .99f)
            {
                ctx.player.IsAttacking = false;
                ctx.ChangeCombatState(nameof(NoneState));
            }
        }
    }

    public void Exit()
    {
    }
}
