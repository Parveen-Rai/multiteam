using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamTangle.CharacterController
{

    public class Player : MonoBehaviour
    {
        public PlayerStats playerStats;

        public Collider2D feetCollider;

        float jumpBufferCounter = 0f;

        float coyoteTimeCounter = 0f;

        private float verticalVelocity = 0f;

        private float horizontalVelocity = 0f;

        private bool isGrounded = false;

        private Transform currentGround;

        private Transform currentGroundPosition;

        private Vector3 lastGroundedPosition;

        private PlayerAdept playerAdept;

        private RaycastHit2D raycastHit2D;

        void Start()
        {
            playerAdept = GetComponent<PlayerAdept>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckforGround();
            if (playerStats.canControl)
            {
                Jump();
                handleHorizontalMovement();
            }
            snapToGround();
            applyGravity();
            Move();
            followGround();

        }

        void applyGravity()
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity += playerStats.gravity * playerStats.fallMultiplier * Time.deltaTime;
            }
            else
            {
                verticalVelocity += playerStats.gravity * Time.deltaTime;
            }
        }

        void CheckforGround()
        {
            raycastHit2D = Physics2D.BoxCast(feetCollider.bounds.center, feetCollider.bounds.size, 0f, Vector2.down, playerStats.groundCheckLength, playerStats.groundLayer);
            
            // Only consider grounded if we're moving downward or stationary (not mid-jump)
            if(raycastHit2D.collider != null && verticalVelocity <= 0.1f)
            {
                isGrounded = true;
                currentGround = raycastHit2D.collider.transform;
            }
            else
            {
                isGrounded = false;
                currentGround = null;
            }
        }


        void snapToGround()
        {
            if (isGrounded && verticalVelocity <= 0 && raycastHit2D.collider != null)
            {
                // Calculate distance to ground
                float distanceToGround = raycastHit2D.distance;
                
                // Only snap if we're very close (prevents snapping from far away)
                if(distanceToGround < 0.05f)
                {
                    verticalVelocity = 0f;
                    float targetY = raycastHit2D.collider.bounds.max.y + playerStats.playerHeight / 2;
                    transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(feetCollider.bounds.center + Vector3.down * playerStats.groundCheckLength, feetCollider.bounds.size);
        }

        void followGround()
        {
            if (currentGround != null && currentGround.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
            {
                Vector3 groundMovement = currentGroundPosition.position - lastGroundedPosition;
                Debug.Log("Ground Movement: " + groundMovement);
                transform.position += new Vector3(groundMovement.x, 0, 0);
            }

            if (currentGround != null)
            {
                lastGroundedPosition = currentGroundPosition.position;
            }
        }

        void handleHorizontalMovement()
        {
            Vector2 moveInput = playerAdept.Move;
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, moveInput.x * playerStats.moveSpeed, playerStats.acceleration * Time.deltaTime);
            //transform.position += new Vector3(horizontalVelocity, 0, 0) * Time.deltaTime;
        }

        void Move()
        {
            Vector3 velocity = new Vector3(horizontalVelocity, verticalVelocity, 0);
            transform.position += velocity * Time.deltaTime;
        }

        void Jump()
        {
            if (isGrounded)
            {
                coyoteTimeCounter = playerStats.coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (playerAdept.JumpPressed)
            {
                jumpBufferCounter = playerStats.jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
            {
                verticalVelocity = playerStats.jumpForce;
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
        }
    }
}
