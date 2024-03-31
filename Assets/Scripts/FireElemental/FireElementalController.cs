using System;
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
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Animator _anim;
        private Vector2 _moveInput;
        private bool _isMoving = false;

        public bool IsMoving
        {
            get
            {
                return _isMoving;
            }
            private set
            {
                _isMoving = value;
                _anim.SetBool("isMoving", value);
            }
        }

        // **************** JUMP - RELATED *****************
        [SerializeField] private float jumpPower;
        // *************************************************

        // **************** BALOON - RELATED *****************
        [SerializeField] private LayerMask baloonLayer;
        private bool _onBaloon;
        // *************************************************

        // **************** GROUND - CHECK *****************
        private bool _isGrounded;
        [SerializeField] private Transform feetPos;
        [SerializeField] private float castDistance;
        [SerializeField] private LayerMask groundLayer;
        // *************************************************

        public static UnityEvent InteractPressed;

        private void Awake()
        {
            if (_controls == null)
            {
                _controls = new FireElementalControls();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();

            spriteRenderer = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            InteractPressed = new UnityEvent();
            _fireElRb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {

        }

        void Update()
        {
            _fireElRb.velocity = new Vector2((_controls.Gameplay.Move.ReadValue<Vector2>() * moveSpeed).x, _fireElRb.velocity.y);
            // **************** GROUND - CHECK *****************
            _isGrounded = Physics2D.OverlapCircle(feetPos.position, castDistance, groundLayer);
            // *************************************************

            _onBaloon = Physics2D.OverlapCircle(feetPos.position, castDistance, baloonLayer);
        }

        public static void Death()
        {
            throw new NotImplementedException();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            IsMoving = _moveInput != Vector2.zero;

            spriteRenderer.flipX = _moveInput.x < 0;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!_isGrounded || _onBaloon)
            {
                return;
            }
            
            _fireElRb.velocity = new Vector2(_fireElRb.velocity.x, jumpPower);
        }

        private void OnDrawGizmos()
        {
            var bounds = GetComponent<Collider2D>().bounds;
            Debug.DrawRay(bounds.center, new Vector3(0, -bounds.extents.y + -castDistance));
            Gizmos.DrawWireCube(transform.position,bounds.size);
        }

        public void OnInteract(InputAction.CallbackContext context)
        { 
            InteractPressed.Invoke();
        }
    }
}
