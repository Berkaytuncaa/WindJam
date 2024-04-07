using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

        void Start()
        {

        }

        void Update()
        {
            
            isGrounded = IsGrounded();
            if (_fireElRb.velocity.y >= 0)
            {
                _fireElRb.gravityScale = defaultGravityScale;
            }
            else
            {
                _fireElRb.gravityScale = fallingGravityScale;
            }
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
            _fireElRb.velocity = new Vector2((_controls.Gameplay.Move.ReadValue<Vector2>() * moveSpeed).x, _fireElRb.velocity.y);

            Debug.Log(context.phase);
            _moveInput = context.ReadValue<Vector2>();

            _spriteRenderer.flipX = _moveInput.x < 0;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started when !isGrounded || _onBalloon:
                    return;
                case InputActionPhase.Started:
                    _fireElRb.velocity = new Vector2(_fireElRb.velocity.x, jumpPower);
                    break;
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        { 
            InteractPressed.Invoke();
        }
    }
}
