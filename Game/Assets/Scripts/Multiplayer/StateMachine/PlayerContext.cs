using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext
{

    public PlayerContext() { }
    public PlayerContext(StateMachine machine)
    {
        this.locomotionMachine = machine;
    }
    public PlayerContext SetController(Fusion.NetworkCharacterController controller)
    {
        this.controller = controller;
        return this;
    }
    public PlayerContext SetAnimController(PlayerAnimatorController anim)
    {
        this.anim = anim;
        return this;
    }
    public PlayerContext SetMovementStateMachine(StateMachine machine)
    {
        this.locomotionMachine = machine;
        return this;
    }

    public PlayerContext SetCombatStateMachine(StateMachine machine)
    {
        this.combatMachine = machine;
        return this;
    }
    public PlayerContext AddState(string key, IState value)
    {
        playerStates.Add(key, value);
        return this;
    }

    public PlayerContext SetHPHandler(HPHandler health)
    {
        this.health = health;
        return this;
    }
    public void SetInput(NetworkInputData inputData)
    {
        this.inputData = inputData;
    }

    public IState GetState(string key)
    {

        return playerStates[key];
    }   
    public void ChangeMovementState(string state)
    {
        var locomotionState = Enum.Parse<LocomotionState>(state);
        player.ChangeLocomotionState(locomotionState);
        locomotionMachine.ChangeState(GetState(state));
    }
    public void ChangeCombatState(string state)
    {
        var combatState = Enum.Parse<CombatState>(state);
        player.ChangeCombatState(combatState);
        Debug.Log(combatState.ToString());
        combatMachine.ChangeState(GetState(state));
    }
    public void SetPlayer(NetworkPlayer player)
    {
        this.player = player;
    }

    public PlayerAnimatorController anim;
    public Fusion.NetworkCharacterController controller;
    public NetworkPlayer player;
    public HPHandler health;
    public NetworkInputData inputData;
    public StateMachine locomotionMachine;
    public StateMachine combatMachine;
    public Dictionary<string, IState> playerStates = new();
   

}
