using Unity.VisualScripting;
using UnityEngine;

namespace TeamTangle.CharacterController
{

    public class Player : MonoBehaviour
    {

        public bool useAdept = true;
        public float playerHeight = 1f;
        public float rayCastOffset = 0.5f;
        public float moveSpeed = 5f;

        public float acceleration = 10f;
        public float gravity = -9.81f;

        public float fallMultiplier = 2.5f;


        public LayerMask groundLayer;
        public float groundCheckLength = 0.1f;
        public float jumpForce = 5f;

        public float jumpBufferTime = 0.2f;
        float jumpBufferCounter = 0f;

        public float coyoteTime = 0.2f;
        float coyoteTimeCounter = 0f;

        private float verticalVelocity = 0f;

        private float horizontalVelocity = 0f;

        private bool isGrounded = false;

        private Transform currentGround;

        private Vector3 lastGroundedPosition;

        private PlayerAdept playerAdept;

        void Start()
        {
            playerAdept = GetComponent<PlayerAdept>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckforGround();
            followGround();
            if (useAdept)
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
                verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }

        void CheckforGround()
        {
            Vector2 leftFoot = new Vector2(transform.position.x - rayCastOffset, transform.position.y - rayCastOffset);
            Vector2 rightFoot = new Vector2(transform.position.x + rayCastOffset, transform.position.y - rayCastOffset);

            RaycastHit2D leftHit = Physics2D.Raycast(leftFoot, Vector2.down, groundCheckLength, groundLayer);
            RaycastHit2D rightHit = Physics2D.Raycast(rightFoot, Vector2.down, groundCheckLength, groundLayer);

           isGrounded  = false;
           if(leftHit.collider != null && leftHit.collider.gameObject != gameObject)
            {
                isGrounded = true;
                currentGround = leftHit.collider.transform;
                
            }
            else if (rightHit.collider != null && rightHit.collider.gameObject != gameObject)
            {
                isGrounded = true;
                currentGround = rightHit.collider.transform;
            }
            else
            {
                isGrounded = false;
                currentGround = null;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 leftFoot = new Vector2(transform.position.x - rayCastOffset, transform.position.y - rayCastOffset);
            Vector2 rightFoot = new Vector2(transform.position.x + rayCastOffset, transform.position.y - rayCastOffset);

            Gizmos.DrawLine(leftFoot, leftFoot + Vector2.down * groundCheckLength);
            Gizmos.DrawLine(rightFoot, rightFoot + Vector2.down * groundCheckLength);

            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * playerHeight / 2);
        }

        void snapToGround()
        {
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = 0f;


                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, playerHeight / 2, groundLayer);
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    float top = hit.collider.bounds.max.y;
                    transform.position = new Vector3(transform.position.x, top + playerHeight / 2, transform.position.z);
                }

            }
        }

        void followGround()
        {
           if(currentGround != null && currentGround.gameObject.layer == LayerMask.NameToLayer("PlayerHead"))
           {
               Vector3 groundMovement = currentGround.position - lastGroundedPosition;
               transform.position += new  Vector3(groundMovement.x, 0, 0);
           }

           if(currentGround != null)
            {
                lastGroundedPosition = currentGround.position;
            }
        }

        void handleHorizontalMovement()
        {
            Vector2 moveInput = playerAdept.Move;
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, moveInput.x * moveSpeed, acceleration * Time.deltaTime);
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
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (playerAdept.JumpPressed)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
            {
                verticalVelocity = jumpForce;
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
        }
    }
}
