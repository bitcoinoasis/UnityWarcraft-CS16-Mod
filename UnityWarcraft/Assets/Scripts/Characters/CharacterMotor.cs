using UnityEngine;

namespace Warcraft.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField, Min(0f)] private float walkSpeed = 4.5f;
        [SerializeField, Min(0f)] private float sprintSpeed = 6.5f;
        [SerializeField, Min(0f)] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -19.6f;
        [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 85f;

        private CharacterController _controller;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private Vector3 _velocity;
        private bool _shouldJump;
        private bool _isSprinting;
        private float _pitch;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraPivot == null)
            {
                cameraPivot = transform;
            }
        }

        public void SetMoveInput(Vector2 move, bool sprint)
        {
            _moveInput = Vector2.ClampMagnitude(move, 1f);
            _isSprinting = sprint;
        }

        public void SetSprinting(bool sprint)
        {
            _isSprinting = sprint;
        }

        public Vector2 GetCurrentMoveInput() => _moveInput;

        public void SetLookInput(Vector2 lookDelta)
        {
            _lookInput = lookDelta;
        }

        public void QueueJump()
        {
            _shouldJump = true;
        }

        private void Update()
        {
            HandleLook();
            HandleMovement(Time.deltaTime);
        }

        private void HandleLook()
        {
            var yaw = _lookInput.x * lookSensitivity;
            var pitchDelta = -_lookInput.y * lookSensitivity;

            transform.Rotate(Vector3.up, yaw, Space.Self);

            _pitch = Mathf.Clamp(_pitch + pitchDelta, -maxLookAngle, maxLookAngle);
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        private void HandleMovement(float deltaTime)
        {
            var moveSpeed = _isSprinting ? sprintSpeed : walkSpeed;
            var move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            var displacement = move * moveSpeed;

            if (_controller.isGrounded)
            {
                _velocity.y = -2f; // keep grounded

                if (_shouldJump)
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }

            _shouldJump = false;

            _velocity.y += gravity * deltaTime;

            _controller.Move((displacement + _velocity) * deltaTime);
        }
    }
}
