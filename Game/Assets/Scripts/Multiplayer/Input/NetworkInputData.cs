using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
    public Vector3 pitchYaw;
    public float rotationCamera;
    public bool isJump;
    public bool isAttack;
    public bool isLeftShift;
    public bool isDefend;
}
