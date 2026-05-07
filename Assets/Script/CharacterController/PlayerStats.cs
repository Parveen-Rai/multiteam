using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public bool canControl = true;

    public float playerHeight = 1f;

    [Range(5f, 20f)]
    public float moveSpeed = 5f;

    [Range(5f, 20f)]
    public float acceleration = 10f;

    [Range(-20f, -5f)]
    public float gravity = -9.81f;

    [Range(1f, 5f)]
    public float fallMultiplier = 2.5f;

    public LayerMask groundLayer;

    public float groundCheckLength = 0.1f;

    [Range(1f, 10f)]
    public float jumpForce = 5f;

    [Range(0.1f, 1f)]
    public float jumpBufferTime = 0.2f;

    [Range(0.1f, 1f)]
    public float coyoteTime = 0.2f;

    

}
