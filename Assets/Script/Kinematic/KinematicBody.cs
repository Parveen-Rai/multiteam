using UnityEngine;

public class KinematicBody : MonoBehaviour
{
    protected float verticalVelocity = 0f;

    protected float horizontalVelocity = 0f;

    protected bool isGrounded = false;

    protected Transform currentGround = null;

    protected RaycastHit2D groundHit2d;

    protected float groundStickTimer = 0f;

    protected Vector3 totalVelocity = Vector3.zero;

    protected virtual void ApplyGravity(float gravity, float fallMultiplier)
    {
        if (verticalVelocity < 0)
        {
            verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    protected virtual void Move() { }

    protected virtual void CheckforGround(Collider2D feetCollider, float groundCheckLength, LayerMask groundLayer , float groundStickTime)
    {
        groundHit2d = Physics2D.BoxCast(
              feetCollider.bounds.center,
              feetCollider.bounds.size,
              0f,
              Vector2.down,
              groundCheckLength,
              groundLayer
          );

        if (groundHit2d.collider != null)
        {
            isGrounded = true;

            groundStickTimer = groundStickTime;

            currentGround = groundHit2d.collider.transform.root;
        }
        else
        {
            groundStickTimer -= Time.deltaTime;

            if (groundStickTimer <= 0f)
            {
                isGrounded = false;
                currentGround = null;
            }
        }
    }

    protected virtual void snapToGround( float height)
    {
        if (isGrounded && verticalVelocity <= 0 && groundHit2d.collider != null)
        {
            // Calculate distance to ground
            float distanceToGround = groundHit2d.distance;

            // Only snap if we're very close (prevents snapping from far away)
            if (distanceToGround < 0.05f)
            {
                verticalVelocity = 0f;
                float targetY = groundHit2d.collider.bounds.max.y + height / 2;
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
            }
        }
    }
}
