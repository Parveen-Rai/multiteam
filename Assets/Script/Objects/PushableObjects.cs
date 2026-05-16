using TeamTangle.CharacterController;
using UnityEngine;

public class PushableObjects : KinematicBody
{
    public Collider2D objectBottomCollider;
    public LayerMask groundCollisionLayer;
    private float Gravity = -20f;

    private float fallMultiplier = 2.5f;

 
    void Update()
    {
        CheckforGround(objectBottomCollider, 0f, groundCollisionLayer, 0.1f);
        snapToGround(1f);
        ApplyGravity(Gravity, fallMultiplier);
        Move();
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
                float moveX = 0f;

            totalVelocity = new Vector3(
                moveX / Time.deltaTime,
                verticalVelocity,
                0
            ) + inheritedVelocity;

            transform.position += totalVelocity * Time.deltaTime;
    }
}
