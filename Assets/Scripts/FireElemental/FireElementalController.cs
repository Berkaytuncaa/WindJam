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


        // **************** JUMP - RELATED *****************
        [SerializeField] private float jumpPower;
        // *************************************************

        // **************** BALOON - RELATED *****************
        private bool _onBalloon;
        // *************************************************

        // **************** GROUND - CHECK *****************
        private bool _isGrounded;
        [SerializeField] private float castDistance;
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private GameObject boxRef;
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
            _isGrounded = IsGrounded();
        }

        private bool IsGrounded()
        {
            Vector2 boxSize = new(.25f,.01f);
            bool hit2D = Physics2D.BoxCast(
                transform.position - new Vector3(0, spriteRenderer.bounds.extents.y + boxSize.y + .01f, 0), boxSize,
                0, Vector2.down, boxSize.y);

            //visualization
            boxRef.transform.position =
                transform.position - new Vector3(0, spriteRenderer.bounds.extents.y + boxSize.y + .01f, 0);
            boxRef.transform.localScale = boxSize;

            return hit2D;
        }

        public static void Death()
        {
            throw new NotImplementedException();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();

            spriteRenderer.flipX = _moveInput.x < 0;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!_isGrounded || _onBalloon)
            {
                return;
            }
            
            _fireElRb.velocity = new Vector2(_fireElRb.velocity.x, jumpPower);
        }

        public void OnInteract(InputAction.CallbackContext context)
        { 
            InteractPressed.Invoke();
        }
    }
}
