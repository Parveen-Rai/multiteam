using TeamTangle.CharacterController;
using UnityEngine;

public class PushableObjects : KinematicBody
{
    public Collider2D objectBottomCollider;
    public LayerMask groundCollisionLayer;

    public float pushResistance = 1f;
    private float Gravity = -20f;

    private float fallMultiplier = 2.5f;

    private bool isPushed = false;

 
    void Update()
    {
        CheckforGround(objectBottomCollider, 0f, groundCollisionLayer, 0.1f);
        snapToGround(0.8f);
        ApplyGravity(Gravity, fallMultiplier);
        Move();
    }

    public void push(float horizontalPush)
    {
        isPushed = horizontalPush != 0f;
        if(isPushed)
        {
            horizontalVelocity = horizontalPush / pushResistance;
        }
        else
        {
            horizontalVelocity = 0f;
        }
    }

    override protected void Move()
    {
        Vector3 inheritedVelocity = Vector3.zero;

            // if (currentGround != null)
            // {
            //     Player groundPlayer = currentGround.GetComponent<Player>();

            //     if (groundPlayer != null)
            //     {
            //         inheritedVelocity = groundPlayer.totalVelocity;
            //     }
            // }

            // float moveX = horizontalVelocity * Time.deltaTime;

            // moveX = ResolveHorizontalCollisions(moveX);
                float moveX = horizontalVelocity * Time.deltaTime;

            totalVelocity = new Vector3(
                moveX / Time.deltaTime,
                verticalVelocity,
                0
            ) + inheritedVelocity;

            transform.position += totalVelocity * Time.deltaTime;
    }
}
