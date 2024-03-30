using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FireElemental
{
    public class FireElementalController : MonoBehaviour, FireElementalControls.IGameplayActions
    {
        private FireElementalControls _controls;
        private Rigidbody2D _fireElRb;
        [SerializeField] private float jumpPower;
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
            _fireElRb = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _fireElRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}
