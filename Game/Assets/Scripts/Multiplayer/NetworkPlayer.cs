using System;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, ITakeDamageable
{

    [SerializeField] Transform _camera;
    [SerializeField] Transform cinemachine;
    [SerializeField] Animator animator;
    [SerializeField] NetworkCharacterController controller;
    [SerializeField] HPHandler hphandler;
    [SerializeField] TextMeshProUGUI text;
    [Networked, OnChangedRender(nameof(OnLocomotionStateChange))]
    public LocomotionState locomotionState { get; set; }
    [Networked, OnChangedRender(nameof(OnCombatStateChanged))]
    public CombatState combatState { get; set; }

    [Networked]
    public bool IsAttacking { get; set; }
    public static NetworkPlayer Local;
    StateMachine locomotion;
    StateMachine combat;
    PlayerContext ctx;
    [Networked, OnChangedRender(nameof(OnNickNameChanged))]
    public NetworkString<_32> nickName { get; set; }

    public void OnNickNameChanged()
    {
        Debug.Log(gameObject.name + ": OnNickNameChanged");
        if(text != null)
        text.text = nickName.ToString();
    }


    private void Awake()
    {
        Debug.Log("NetworkPlayer Awake");
        locomotion = new StateMachine();
        combat = new StateMachine();
        ctx = new PlayerContext();
        ctx.SetAnimController(new PlayerAnimatorController(animator))
           .SetHPHandler(hphandler)
           .SetController(controller)
           .SetMovementStateMachine(locomotion)
           .SetCombatStateMachine(combat)
           .AddState(nameof(Idle), new Idle(ctx))
           .AddState(nameof(Move), new Move(ctx))
           .AddState(nameof(Attack01), new Attack01(ctx))
           .AddState(nameof(Attack02), new Attack02(ctx))
           .AddState(nameof(JumpStart), new JumpStart(ctx))
           //.AddState(nameof(JumpEnd), new JumpEnd(ctx))
           .AddState(nameof(Fall), new Fall(ctx))
           .AddState(nameof(Defend), new Defend(ctx))
           .AddState(nameof(Die), new Die(ctx))
           .AddState(nameof(NoneState), new NoneState(ctx))
           .AddState(nameof(DefendHit), new DefendHit(ctx))
           .AddState(nameof(GetHit), new GetHit(ctx))
           ;
        hphandler.OnDead += OnDeadHPHandler;
    }
    public void OnGetHit()
    {
        if (hphandler.IsDead)
        {
            return;
        }
        if (combatState == CombatState.Defend)
        {
            ctx.ChangeCombatState(nameof(DefendHit));
        }
        else
        {
            ctx.ChangeMovementState(nameof(GetHit));
        }
    }

    void OnDeadHPHandler()
    {
        //ctx.ChangeCombatState(nameof(NoneState));
        //ctx.ChangeMovementState(nameof(Die));
        animator.Play(nameof(NoneState), 1);
        animator.Play(nameof(Die), 0);
    }
    public override void Spawned()
    {
        Debug.Log("NetworkPlayer spawned");
        ctx.SetPlayer(this);
        locomotion.ChangeState(ctx.GetState(nameof(Idle)));
        combat.ChangeState(ctx.GetState(nameof(NoneState)));
        if (Object.HasInputAuthority)
        {
            Local = this;
            _camera.gameObject.SetActive(true);
            cinemachine.gameObject.SetActive(true);
            
                Debug.Log(gameObject.name + ": PlayerEelemntRoom Spawned on client");
                Utils.Delay1Frame(() => RPC_RequestUserName(NetworkRunnerHandler.Ins.nickNamePlayer));
            
        }
        else
        {
            Debug.Log("Destroy camera");
            Destroy(_camera.gameObject);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestUserName(string userName)
    {
        Debug.Log(gameObject.name + ": RequestUserID called");
        this.nickName = userName;
    }

    NetworkInputData inputData;

    [Networked]
    public bool isJumping { get; set; }    
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && hphandler.IsDead)
        {
            return;
        }
        if (GetInput(out inputData))
        {
            ctx.SetInput(inputData);
            MoveHandle(inputData);                  
            if (inputData.isJump && controller.Grounded && !isJumping)
            {
                ctx.ChangeMovementState(nameof(JumpStart));
            }
            if (inputData.isAttack && IsAttacking == false)
            {
                ctx.ChangeCombatState(nameof(Attack01));
                IsAttacking = true;
            }
            if (inputData.isDefend)
            {
                ctx.ChangeCombatState(nameof(Defend));               
            }
            
            locomotion.Update(Runner.DeltaTime);
            combat.Update(Runner.DeltaTime);
            CheckFallRespawn();
        }
        else
        {
            inputData.Reset();
            ctx.SetInput(inputData);
            MoveHandle(inputData);
            locomotion.Update(Runner.DeltaTime);
            combat.Update(Runner.DeltaTime);
        }
           
    }
    void OnLocomotionStateChange()
    {
        switch (locomotionState)
        {
            case LocomotionState.Idle:
                ctx.anim.PlayClip(PlayerAnimatorController.IDLE_HASH);
                break;
            case LocomotionState.Move:
                ctx.anim.PlayClip(PlayerAnimatorController.MOVE_HASH);
                break;
            case LocomotionState.JumpStart:
                ctx.anim.PlayClip(PlayerAnimatorController.JUMPSTART_HASH);
                break;
            case LocomotionState.Fall:
                ctx.anim.PlayClip(PlayerAnimatorController.FALL_HASH);
                break;
            case LocomotionState.JumpEnd:
                //ctx.anim.PlayClip(PlayerAnimatorController.JUMPEND_HASH);
                break;
            case LocomotionState.Die:
                ctx.anim.PlayClip(PlayerAnimatorController.DIE_HASH);
                break;            
            case LocomotionState.GetHit:
                ctx.anim.PlayClip(PlayerAnimatorController.GETHIT_HASH);
                break;
            case LocomotionState.GetUp:
                ctx.anim.PlayClip(PlayerAnimatorController.GETUP_HASH);
                break;
            default:
                break;

        }
    }
    void OnCombatStateChanged()
    {
        switch (combatState) 
        {
            case CombatState.NoneState:
                ctx.anim.PlayClip(PlayerAnimatorController.NONE_HASH, 1);
                break;
            case CombatState.Attack01:
                ctx.anim.PlayClip(PlayerAnimatorController.ATTACK01_HASH, 1);
                break;
            case CombatState.Attack02:
                ctx.anim.PlayClip(PlayerAnimatorController.ATTACK02_HASH, 1);
                break;
            case CombatState.Defend:
                ctx.anim.PlayClip(PlayerAnimatorController.DEFEND_HASH, 1);
                break;
            case CombatState.DefendHit:
                ctx.anim.PlayClip(PlayerAnimatorController.DEFEND_HASH, 1);
                break;
            default:
                break;
        }

    }
    public void ChangeLocomotionState(LocomotionState state)
    {
        if(HasStateAuthority)
            locomotionState = state;
    }
    public void ChangeCombatState(CombatState state)
    {
        if (HasStateAuthority)
        {
            combatState = state;
        }
    }
    public override void Render()
    {
        //animator.Play();
    }

    void MoveHandle(NetworkInputData inputData)
    {
        float maxSpeed = inputData.isLeftShift ? 8 : 10;
        float speed = inputData.isDefend ? maxSpeed * .7f : maxSpeed;

        controller.maxSpeed = speed;

        Vector2 dir = inputData.direction;

        Vector3 movDir = Vector3.zero;

        if (dir.sqrMagnitude > 0.001f)
        {
            float targetAngle =
                Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg
                + inputData.rotationCamera;

            movDir =
                Quaternion.Euler(0, targetAngle, 0)
                * Vector3.forward;
        }

        controller.Move(movDir);
        animator.SetFloat("moveSpeed", movDir.magnitude);
    }

    public void TakeDamage(float damage)
    {
        if (!HasStateAuthority) return;
        if (combatState == CombatState.Defend)
        {
            damage -= 5f;
            //ChangeDefendHit          
        }        
        hphandler.TakeDamage(damage);
    }

    void CheckFallRespawn()
    {
        if(transform.position.y < -10f)
        {
            if (Object.HasStateAuthority && GameManager.Ins != null)
            {
                controller.Teleport(Utils.GetRandomAroundPoint(GameManager.Ins.SpawnPoint));
            }
        }
    }
}
