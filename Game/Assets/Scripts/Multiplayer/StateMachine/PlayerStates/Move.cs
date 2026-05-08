using UnityEngine;

public class Move : IState
{
    PlayerContext ctx;
    Animator animator;
    public Move(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    Transform player;
    public void Enter()
    {

        Debug.Log("Enter MoveState");
        if(player == null)
        {
            player = ctx.controller.transform;
        }
        if (animator == null) {
            animator = ctx.anim.GetAnimator();
        }
        ctx.anim.PlayClip(PlayerAnimatorController.MOVE_HASH);
    }
    public void Execute(float tick)
    {
        if(ctx.inputData.direction != Vector2.zero)
        {
          

            float maxSpeed = ctx.inputData.isLeftShift ? 15 : 20;
            float speed = ctx.inputData.isDefend ? maxSpeed * .7f : maxSpeed; // giam 30% toc do khi vua di chuyen vua defend
            ctx.controller.maxSpeed = speed;
            animator.SetFloat("moveSpeed", speed/maxSpeed);
            Vector2 dir = ctx.inputData.direction;
            float targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg + ctx.inputData.rotationCamera;
            Vector3 movDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            Debug.Log("dirInput: " + dir);
            Debug.Log("ctx.RotationCamera = " + ctx.inputData.rotationCamera);
            Debug.Log("movDir: " + movDir);
            ctx.controller.Move(movDir);
        }
        else
        {
            ctx.ChangeMovementState(nameof(Idle));
        }
        
    }

    public void Exit()
    {
    }

  
}
