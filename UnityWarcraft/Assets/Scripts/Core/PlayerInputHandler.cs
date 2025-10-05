using UnityEngine;
using UnityEngine.InputSystem;
using Warcraft.Characters;

namespace Warcraft.Core
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private CharacterMotor motor;
        [SerializeField] private CharacterCombat combat;

        private bool _isSprinting;

        private void Reset()
        {
            motor = GetComponent<CharacterMotor>();
            combat = GetComponent<CharacterCombat>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (motor == null)
            {
                return;
            }

            var move = context.ReadValue<Vector2>();
            motor.SetMoveInput(move, _isSprinting);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (motor == null)
            {
                return;
            }

            var look = context.ReadValue<Vector2>();
            motor.SetLookInput(look);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed || motor == null)
            {
                return;
            }

            motor.QueueJump();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            _isSprinting = context.ReadValueAsButton();
            if (motor != null)
            {
                motor.SetSprinting(_isSprinting);
                motor.SetMoveInput(motor.GetCurrentMoveInput(), _isSprinting);
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                combat?.FirePrimary();
            }
        }

        public void OnSecondary(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                combat?.FireSecondary();
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                combat?.Reload();
            }
        }
    }
}
