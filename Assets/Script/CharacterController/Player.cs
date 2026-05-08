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

        private PlayerAdept playerAdept;

        private RaycastHit2D raycastHit2D;

        private Player groundPlayer;

        private float groundStickTimer = 0f;

        private Vector3 totalVelocity = Vector3.zero;



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
            raycastHit2D = Physics2D.BoxCast(
                feetCollider.bounds.center,
                feetCollider.bounds.size,
                0f,
                Vector2.down,
                playerStats.groundCheckLength,
                playerStats.groundLayer
            );

            if (raycastHit2D.collider != null)
            {
                isGrounded = true;

                groundStickTimer = playerStats.groundStickTime;

                currentGround = raycastHit2D.collider.transform.root;
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


        void snapToGround()
        {
            if (isGrounded && verticalVelocity <= 0 && raycastHit2D.collider != null)
            {
                // Calculate distance to ground
                float distanceToGround = raycastHit2D.distance;

                // Only snap if we're very close (prevents snapping from far away)
                if (distanceToGround < 0.05f)
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

        void handleHorizontalMovement()
        {
            Vector2 moveInput = playerAdept.Move;
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, moveInput.x * playerStats.moveSpeed, playerStats.acceleration * Time.deltaTime);
        }

        void Move()
        {
            Vector3 inheritedVelocity = Vector3.zero;

            if (currentGround != null)
            {
                Player groundPlayer = currentGround.GetComponent<Player>(); 
                if(groundPlayer != null)
                {
                    inheritedVelocity = groundPlayer.totalVelocity;
                }
            }

            totalVelocity    = new Vector3(horizontalVelocity, verticalVelocity, 0) + inheritedVelocity;
            transform.position += totalVelocity * Time.deltaTime;
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
