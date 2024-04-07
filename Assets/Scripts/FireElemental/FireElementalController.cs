using System;
using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FireElemental
{
    public class FireElementalController : MonoBehaviour, FireElementalControls.IGameplayActions
    {
        private FireElementalControls _controls;
        private Rigidbody2D _fireElRb;
        [SerializeField] private float moveSpeed;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _moveInput;

        #region Jump 
        
        [ReadOnly][SerializeField] private bool isGrounded;
        [SerializeField] private float jumpPower;
        [SerializeField] private float defaultGravityScale;
        [SerializeField] private float fallingGravityScale;
        [ReadOnly] [SerializeField] private bool isJumping;
        [SerializeField] private float maxJumpTimeBound;
        
        #endregion 

        #region Balloon

        private bool _onBalloon;

        #endregion

        public static UnityEvent InteractPressed;

        private void Awake()
        {
            if (_controls == null)
            {
                _controls = new FireElementalControls();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            InteractPressed = new UnityEvent();
            _fireElRb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            isGrounded = IsGrounded();
            //_fireElRb.gravityScale = _fireElRb.velocity.y >= 0 ? defaultGravityScale : fallingGravityScale;
        }

        private bool IsGrounded()
        {
            Vector2 boxSize = new(.25f,.01f);
            bool hit2D = Physics2D.BoxCast(
                transform.position - new Vector3(0, _collider.bounds.extents.y + boxSize.y , 0), boxSize,
                0, Vector2.down, boxSize.y + .1f, LayerMask.GetMask("Ground"));
            
            return hit2D;
        }

        public static void Death()
        {
            throw new NotImplementedException();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                StartCoroutine(nameof(MoveRoutine));
            }
            else
            {
                _fireElRb.velocity = new Vector2(0, _fireElRb.velocity.y);
                StopCoroutine(nameof(MoveRoutine));
            }

            _moveInput = context.ReadValue<Vector2>();

            _spriteRenderer.flipX = _moveInput.x < 0;
        }
        
        //absolutely no need to use this, it's just for my fantasy fulfillment
        private IEnumerator MoveRoutine()
        {
            while (true)
            {
                _fireElRb.velocity = new Vector2((_controls.Gameplay.Move.ReadValue<Vector2>() * moveSpeed).x, _fireElRb.velocity.y);
                yield return new WaitForFixedUpdate();
            }
            // ReSharper disable once IteratorNeverReturns
            // cuz I couldn't figure out any other way to break the loop other than StopCoroutine in the outer scope
            // actually a global variable isMoving could be made to keep track of when the while loop should end but... nah
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started when !isGrounded || _onBalloon:
                    return;
                case InputActionPhase.Started:
                    float jumpForce = Mathf.Sqrt(jumpPower * -2 * (Physics2D.gravity.y * _fireElRb.gravityScale));
                    _fireElRb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    isJumping = true;
                    break;
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Performed:
                    StartCoroutine(nameof(JumpRoutine));
                    break;
                case InputActionPhase.Canceled:
                    isJumping = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator JumpRoutine()
        {
            // create timer in coroutine
            float jumpTime = 0;
            while (isJumping && jumpTime <= maxJumpTimeBound)
            {
               //_fireElRb.velocity = new Vector2(_fireElRb.velocity.x, jumpPower);
               jumpTime += Time.deltaTime;
               yield return new WaitForFixedUpdate();
            }

            _fireElRb.AddForce(Vector2.down * 1000, ForceMode2D.Force);
        }

        public void OnInteract(InputAction.CallbackContext context)
        { 
            InteractPressed.Invoke();
        }
    }
}
