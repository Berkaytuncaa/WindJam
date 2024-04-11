using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace FireElemental
{
    // about the whole movement thing: https://gamedevbeginner.com/how-to-jump-in-unity-with-or-without-physics/
    public class FireElementalController : MonoBehaviour, FireElementalControls.IGameplayActions
    {
        private FireElementalControls _controls;
        private Rigidbody2D _fireElRb;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        private ParticleSystem _particleSystem;
        
        #region Animator

        private Animator _animator;
        private readonly int _isMovingHash = Animator.StringToHash("isMoving");

        #endregion
        

        #region Move

        [Header("Move")] 
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveCancelForce;
        private Vector2 _moveInput;

        #endregion

        #region Jump 
        
        [Header("Jump")]
        [ReadOnly][SerializeField] private bool isGrounded;
        [SerializeField] private float jumpPower;
        [SerializeField] private float defaultGravityScale;
        [SerializeField] private float fallingGravityScale;
        [ReadOnly] [SerializeField] private bool isJumping;
        [SerializeField] private float maxJumpTimeBound;
        [SerializeField] private float jumpCancelForce;
        
        #endregion

        #region Dash

        [Header("Dash")] 
        [ReadOnly] [SerializeField] private bool isDashing;
        [ReadOnly] [SerializeField] private bool canDash = true;
        [SerializeField] private float dashPower;
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashCooldown;

        #endregion

        #region Balloon

        private bool _onBalloon;

        #endregion

        public static UnityEvent InteractPressed;
        private FireElementalControls.IGameplayActions _gameplayActionsImplementation;

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
            _animator = GetComponent<Animator>();

            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _particleSystem.Stop();
            
            StartCoroutine(nameof(HandleGravity));
        }

        private void Update()
        {
            isGrounded = IsGrounded();
        }

        private IEnumerator HandleGravity()
        {
            while (true)
            {
                _fireElRb.gravityScale = _fireElRb.velocity.y >= 0 ? defaultGravityScale : fallingGravityScale;
                yield return new WaitForFixedUpdate();
            }
            // ReSharper disable once IteratorNeverReturns
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
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    _moveInput = context.ReadValue<Vector2>();
                    _spriteRenderer.flipX = _moveInput.x < 0;
                    _animator.SetBool(_isMovingHash, true);
                    StartCoroutine(nameof(MoveRoutine));
                    break;
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Canceled:
                    _moveInput = context.ReadValue<Vector2>();
                    _animator.SetBool(_isMovingHash, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //absolutely no need to use this, it's just for my fantasy fulfillment
        private IEnumerator MoveRoutine()
        {
            while (_moveInput.x != 0)
            {
                _fireElRb.velocity = new Vector2(_moveInput.x * moveSpeed, _fireElRb.velocity.y);
                yield return new WaitForFixedUpdate();
            }

            while (_fireElRb.velocity.x > 0)
            {
                _fireElRb.AddForce(Vector2.left * moveCancelForce, ForceMode2D.Force);
                yield return new WaitForFixedUpdate();
            }

            while (_fireElRb.velocity.x < 0)
            {
                _fireElRb.AddForce(Vector2.right * moveCancelForce, ForceMode2D.Force);
                yield return new WaitForFixedUpdate();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started when !isGrounded || _onBalloon:
                    break;
                case InputActionPhase.Started:
                    float jumpForce = Mathf.Sqrt(jumpPower * -2 * (Physics2D.gravity.y * defaultGravityScale));
                    _fireElRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

            while (_fireElRb.velocity.y > 0)
            {
                _fireElRb.AddForce(Vector2.down * jumpCancelForce, ForceMode2D.Force);
                yield return new WaitForFixedUpdate();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        { 
            InteractPressed.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started when isDashing || !canDash:
                    break;
                case InputActionPhase.Started:
                    StartCoroutine(nameof(DashRoutine));
                    break;
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator DashRoutine()
        {
            _controls.Gameplay.Move.Disable();
            isDashing = true;
            canDash = false;
            StopCoroutine(nameof(HandleGravity));
            _fireElRb.gravityScale = 0;
            
            _particleSystem.Play();
            
            _fireElRb.AddForce((_spriteRenderer.flipX ? Vector2.left : Vector2.right) * dashPower, ForceMode2D.Impulse);
            yield return new WaitForSeconds(dashDuration);
            
            _fireElRb.velocity = Vector2.zero;
            StartCoroutine(nameof(HandleGravity));
            isDashing = false;
            
            _controls.Gameplay.Move.Enable();
            _particleSystem.Stop();

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
