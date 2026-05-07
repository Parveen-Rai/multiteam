using UnityEngine;
using TeamTangle.Input;

namespace TeamTangle.CharacterController
{
    public class PlayerAdept : MonoBehaviour
    {
        private Input_Actions inputAction;

        public Vector2 Move { get; private set; }

        public bool JumpPressed { get; private set; }

        void Awake()
        {
            inputAction = new Input_Actions();
            inputAction.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
            inputAction.Player.Move.canceled += ctx => Move = Vector2.zero;
            inputAction.Player.Jump.performed += ctx => JumpPressed = true;
        }
        
        void OnEnable()
        {
            inputAction.Enable();
        }

        void OnDestroy()
        {
            inputAction.Disable();
        }

        void LateUpdate()
        {
            if(JumpPressed)
            {
                JumpPressed = false;
            }
        }
    }
}
