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
        [SerializeField] private float jumpPower;
        [SerializeField] private float moveSpeed;
        private Collider2D _collider;
        
        [SerializeField] private float castDistance;
        [SerializeField] private LayerMask groundLayer;
        private bool IsGrounded
        {
            get
            {
                if (Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0, Vector2.down, _collider.bounds.extents.y + castDistance, groundLayer))
                {
                    return true;
                }

                return false;
            }
        }


        public static UnityEvent InteractPressed;
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new FireElementalControls();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            InteractPressed = new UnityEvent();
            _fireElRb = GetComponent<Rigidbody2D>();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            _fireElRb.velocity = new Vector2((_controls.Gameplay.Move.ReadValue<Vector2>() * moveSpeed).x, _fireElRb.velocity.y);
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
            if (!IsGrounded)
            {
                return;
            }
            _fireElRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
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
