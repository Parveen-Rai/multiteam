using System;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamTangle.CharacterController
{

    public class Player : KinematicBody
    {
        public PlayerStats playerStats;

        public Collider2D feetCollider;

        public Collider2D bodyCollider;

        float jumpBufferCounter = 0f;

        float coyoteTimeCounter = 0f;

        private PlayerAdept playerAdept;


        private Player groundPlayer;


        void Start()
        {
            playerAdept = GetComponent<PlayerAdept>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckforGround(feetCollider, playerStats.groundCheckLength, playerStats.groundLayer, playerStats.groundStickTime);
            if (playerStats.canControl)
            {
                Jump();
                handleHorizontalMovement();
            }
            snapToGround( playerStats.playerHeight);
            ApplyGravity(playerStats.gravity, playerStats.fallMultiplier);
            Move();
        }
        float ResolveHorizontalCollisions(float moveX)
        {
            if (Mathf.Abs(moveX) < 0.0001f)
                return moveX;

            float skinWidth = 0.07f;

            Vector2 direction = moveX > 0 ? Vector2.right : Vector2.left;

            Vector2 boxSize = new Vector2(
                bodyCollider.bounds.size.x,
                bodyCollider.bounds.size.y
            );

            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                bodyCollider.bounds.center,
                boxSize,
                0f,
                direction,
                Mathf.Abs(moveX) + skinWidth,
                playerStats.HorizontalCollisions
            );

            float allowedMove = moveX;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null)
                    continue;

                // Ignore self
                if (hit.collider.transform.root == transform.root)
                    continue;

                float distance = hit.distance - skinWidth;

                if (distance < Mathf.Abs(allowedMove))
                {
                    allowedMove = Mathf.Sign(moveX) *
                                  Mathf.Max(distance, 0);
                }
            }

            return allowedMove;
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(feetCollider.bounds.center + Vector3.down * playerStats.groundCheckLength, feetCollider.bounds.size);

            Gizmos.color = Color.blue;
            if (playerAdept != null)
            {
                float skinWidth = 0.07f;
                Vector2 boxSize = new Vector2(bodyCollider.bounds.size.x, bodyCollider.bounds.size.y);
                Vector3 direction = playerAdept.Move.x > 0 ? Vector3.right : Vector3.left;
                Gizmos.DrawWireCube(bodyCollider.bounds.center + direction *  (Mathf.Abs(playerAdept.Move.x) + skinWidth), boxSize);
            }
        }

        void handleHorizontalMovement()
        {
            Vector2 moveInput = playerAdept.Move;
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, moveInput.x * playerStats.moveSpeed, playerStats.acceleration * Time.deltaTime);
        }

        protected override void Move()
        {
            Vector3 inheritedVelocity = Vector3.zero;

            if (currentGround != null)
            {
                Player groundPlayer = currentGround.GetComponent<Player>();

                if (groundPlayer != null)
                {
                    inheritedVelocity = groundPlayer.totalVelocity;
                }
            }

            float moveX = horizontalVelocity * Time.deltaTime;

            moveX = ResolveHorizontalCollisions(moveX);

            totalVelocity = new Vector3(
                moveX / Time.deltaTime,
                verticalVelocity,
                0
            ) + inheritedVelocity;

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
