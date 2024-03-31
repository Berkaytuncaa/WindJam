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
        // **************** JUMP - RELATED *****************
        [SerializeField] private float jumpPower;
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
        }

        public static void Death()
        {
            throw new NotImplementedException();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!_isGrounded)
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
